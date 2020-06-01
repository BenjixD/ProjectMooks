using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MoveSelectionPhase : Phase {
    protected BattleField _field {get; set;}

    private float _playerActionCounter;
    private Stack<HeroMenuAction> _heroMenuActions;
    private CommandSelector _commandSelector;

    public MoveSelectionPhase(BattleField field, CommandSelector selector, TurnController controller, string callback) : base(controller, callback) {
        this._field = field;
    	this._playerActionCounter = 0;
        this._heroMenuActions = new Stack<HeroMenuAction>();
        this._commandSelector = selector;
    }

    protected override void OnPhaseStart() {
    	this._playerActionCounter = 0;
        this._heroMenuActions.Clear();
        // Reset Player Commands
        foreach (var player in _field.GetAllFightingEntities()) {
            player.ResetCommand();
        }
    }

    protected override IEnumerator Run() {
        this.ApplyStatusAilments();
        yield return _controller.StartCoroutine(HeroMoveSelection());
    }

    // Helper Coroutines
    IEnumerator HeroMoveSelection() {
        this._heroMenuActions.Push(new HeroMenuAction(MenuState.MOVE));
        InitializeCommandCardActionUI(GetHeroActionChoices(ActionType.BASIC));

        while(true) {
            HeroMenuAction menuAction = this.GetHeroMenuAction();
            MenuState menuState = menuAction.menuState;
            switch (menuState) {
                case MenuState.MOVE:
                    if (Input.GetKeyDown(KeyCode.Z)) {
                        SetHeroAction();
                    }
                    break;

                case MenuState.TARGET:
                    if (Input.GetKeyDown(KeyCode.Z)) {
                        SetHeroTarget();
                    }
                    break;
                case MenuState.MAGIC:
                    if (Input.GetKeyDown(KeyCode.Z)) {
                        SetHeroAction();
                    }
                    break;
                default:
                    break;
            }

            if (_heroMenuActions.Count > 1) {
                // Go back a menu
                if (Input.GetKeyDown(KeyCode.X)) {
                    GoBackToLastHeroAction();
                }            
            }


            _playerActionCounter += Time.deltaTime;
            if(this.checkExecuteTurn()) {
                break;
            }
            yield return null;
        }
    }

    private void InitializeCommandCardActionUI(List<HeroActionChoice> currentHeroChoices) {
        List<string> actionNames = currentHeroChoices.Map((HeroActionChoice choice) => { return choice.choiceName; });
        this.ui.commandCardUI.InitializeCommandSelection(actionNames, 0, this._commandSelector);
    }

    private List<HeroActionChoice> GetHeroActionChoices(ActionType type) {
        FightingEntity heroPlayer = this._field.GetHeroPlayer();
        List<ActionBase> actions = this._field.GetHeroPlayer().GetFilteredActions(type);
        List<HeroActionChoice> currentHeroChoices = new List<HeroActionChoice>();

        foreach (ActionBase action in actions) {
            currentHeroChoices.Add(new HeroActionChoice(action.name, action));
        }

        if (type == ActionType.BASIC) {
            if (_field.GetHeroPlayer().GetFilteredActions(ActionType.MAGIC).Count != 0) {
                currentHeroChoices.Insert(1, new HeroActionChoice("Magic", null));
            }
        }

        HeroMenuAction menuAction = GetHeroMenuAction();
        menuAction.targetIndex = 0;
        menuAction.currentHeroChoices = currentHeroChoices;

        return currentHeroChoices;
    }

    private HeroMenuAction GetHeroMenuAction() {
        return this._heroMenuActions.Peek();
    }

    private void SetHeroAction() {
        HeroMenuAction menuAction = GetHeroMenuAction();
        menuAction.targetIndex = this._commandSelector.GetChoice();
        this.ui.commandCardUI.SetConfirmed();
        menuAction.onBackCallback = this.OnActionChooseBackCallback;

        HeroActionChoice choice = menuAction.currentHeroChoices[menuAction.targetIndex];

        if (choice.choiceName == "Magic" && choice.action == null) {
            this._heroMenuActions.Push(new HeroMenuAction(MenuState.MAGIC));
            InitializeCommandCardActionUI(GetHeroActionChoices(ActionType.MAGIC));
        } else {
            ActionBase heroAction = choice.action;
            if (heroAction.targetInfo.targetTeam != TargetTeam.NONE) {
                List<FightingEntity> possibleTargets = heroAction.GetAllPossibleActiveTargets(_field.GetHeroPlayer());
                switch (heroAction.targetInfo.targetType) {
                    case TargetType.SINGLE:
                        this.ui.targetSelectionUI.InitializeTargetSelectionSingle(possibleTargets, 0, this._commandSelector);
                        break;

                    case TargetType.ALL:
                        this.ui.targetSelectionUI.InitializeTargetSelectionAll(possibleTargets);
                    break;

                    default:

                    break;
                }


                this._heroMenuActions.Push(new HeroMenuAction(MenuState.TARGET));
            } else {
                HeroActionConfirmed();
                GetHeroMenuAction().onBackCallback = this.OnActionChooseBackCallback;
                this._field.GetHeroPlayer().SetQueuedAction(new QueuedAction(this._field.GetHeroPlayer(), heroAction, new List<int>()));
                CheckExecuteTurn();
                this._heroMenuActions.Push(new HeroMenuAction(MenuState.WAITING));
            }
        }
    }

    private void SetHeroTarget() {
        this.HeroActionConfirmed();

        HeroMenuAction menuAction = GetHeroMenuAction();
        menuAction.targetIndex = this._commandSelector.GetChoice();
        menuAction.onBackCallback = this.OnTargetChooseBackCallback;

        HeroMenuAction moveMenuAction = GetPreviousHeroMenuAction();

        ActionBase heroAction = moveMenuAction.currentHeroChoices[moveMenuAction.targetIndex].action;
    
        switch (heroAction.targetInfo.targetType) {
            case TargetType.SINGLE:
                List<FightingEntity> possibleTargets = heroAction.GetAllPossibleActiveTargets(field.GetHeroPlayer());
                FightingEntity target = possibleTargets[menuAction.targetIndex];
                field.GetHeroPlayer().SetQueuedAction(new QueuedAction(field.GetHeroPlayer(), heroAction, new List<int>{target.targetId}  ));
                break;

            case TargetType.ALL:
                List<int> allEnemies = field.enemyParty.GetActiveMembers().Map((Enemy enemy) => enemy.targetId);
                field.GetHeroPlayer().SetQueuedAction(new QueuedAction(field.GetHeroPlayer(), heroAction, allEnemies ));
            break;

            default:
            break;
        }


        this._heroMenuActions.Push(new HeroMenuAction(MenuState.WAITING));
    }

    private void HeroActionConfirmed() {
        this.ui.targetSelectionUI.ClearSelection();
        this.ui.commandCardUI.SetCommandCardUI(CommandCardUIMode.CLOSED);
    }

    private bool CheckExecuteTurn() {
        List<Player> players = _field.playerParty.GetActiveMembers();

        //bool timeOutIfChatTooSlow = (stage.GetHeroPlayer().HasSetCommand() && playerActionCounter >= this.maxTimeBeforeAction);
        bool timeOutIfChatTooSlow = false;
        bool startTurn = timeOutIfChatTooSlow || HasEveryoneEnteredActions();
        if (startTurn) {
            playerActionCounter = 0;
        } else {
            UpdateStateText();
        }
        return startTurn;
    }

    private void UpdateStateText() {
        // Currently not in the UI, but may add something similar later
        /*
        if (!field.GetHeroPlayer().HasSetCommand()) {
            this.ui.SetStateText("Waiting on streamer input");
        } else {
            int timer = (int)(this.maxTimeBeforeAction - playerActionCounter);
            this.ui.SetStateText("Waiting on Chat: " + timer);
        }
        */
    }

    private bool hasEveryoneEnteredActions() {
        foreach (var player in _field.playerParty.GetActiveMembers()) {
            if (player.HasSetCommand() == false) {
                return false;
            }
        }

        return true;
    }

    private void GoBackToLastHeroAction() {
        this._heroMenuActions.Pop();
        HeroMenuAction menuAction = GetHeroMenuAction();
        if (menuAction.onBackCallback != null) {
            menuAction.onBackCallback();
        }
    }
}