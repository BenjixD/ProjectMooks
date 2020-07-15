using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum MenuState {
    MOVE,
    MAGIC,
    TARGET,
    WAITING
}

[System.Serializable]
public class HeroActionChoice {
    public string choiceName = "";
    public ActionBase action = null;

    public HeroActionChoice(string actionName, ActionBase action) {
        this.choiceName = actionName;
        this.action = action;
    }
}

public class HeroMenuAction {

    public MenuState menuState = MenuState.MOVE;
    public int targetIndex;
    public List<HeroActionChoice> currentHeroChoices;

    public UnityAction onBackCallback = null;

    public HeroMenuAction(MenuState state) {
        this.menuState = state;
    }
}

[System.Serializable]
public class MoveSelectionPhase : Phase {
    public float maxTimeBeforeAction = 25f;

    protected BattleUI _ui {get; set; }
    protected BattleField _field {get; set;}

    private float _playerActionCounter;
    private Stack<HeroMenuAction> _heroMenuActions;
    private CommandSelector _commandSelector;

    public MoveSelectionPhase(BattleUI ui, BattleField field, CommandSelector selector, string callback) : base(TurnPhase.MOVE_SELECTION, callback) {
        this._ui = ui;
        this._field = field;
        this._playerActionCounter = 0;
        this._heroMenuActions = new Stack<HeroMenuAction>();
        this._commandSelector = selector;
    }

    protected override void OnPhaseStart() {
        this._playerActionCounter = 0;
        this._heroMenuActions.Clear();
        // Reset Player Commands
        PlayerObject heroPlayer = this._field.GetHeroPlayer();
        foreach (FightingEntity player in this._field.GetAllFightingEntities()) {
            if (player == heroPlayer || player.isEnemy()) {
                player.ResetCommand();
            } else {
                // Experimental: Mooks keep their actions
                QueuedAction lastAction = player.GetQueuedAction();
                if (lastAction != null && lastAction._action != null && lastAction.CanQueueAction(lastAction._action)) {
                    player.SetQueuedAction(lastAction._action, lastAction._targetIds);
                } else {
                    player.ResetCommand();
                }
            }
        }

        Messenger.Broadcast(Messages.OnUpdateStatusBarsUI);
    }

    protected override IEnumerator Run() {
        this.ApplyStatusAilments(this._field.GetAllFightingEntities());
        yield return GameManager.Instance.time.GetController().StartCoroutine(HeroMoveSelection());
    }

    public override bool CanInputActions() {
        return true;
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

            if (Input.GetKeyDown(KeyCode.L)) {
                _playerActionCounter = this.maxTimeBeforeAction;
            }

            if (this._field.GetHeroPlayer().HasSetCommand()) {
                _playerActionCounter += Time.deltaTime;
            }
            
            if(this.CheckExecuteTurn()) {
                break;
            }
            yield return null;
        }
    }

    private void InitializeCommandCardActionUI(List<HeroActionChoice> currentHeroChoices) {
        List<string> actionNames = currentHeroChoices.Map((HeroActionChoice choice) => { return choice.choiceName; });
        this._ui.commandCardUI.InitializeCommandSelection(actionNames, 0, this._commandSelector);
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
        menuAction.onBackCallback = this.OnActionChooseBackCallback;

        HeroActionChoice choice = menuAction.currentHeroChoices[menuAction.targetIndex];

        if (choice.choiceName == "Magic" && choice.action == null) {
            this._heroMenuActions.Push(new HeroMenuAction(MenuState.MAGIC));
            InitializeCommandCardActionUI(GetHeroActionChoices(ActionType.MAGIC));
        } else {
            ActionBase heroAction = choice.action;

            if (!heroAction.CheckCost(this._field.GetHeroPlayer())){
                return;
            }

            this._ui.commandCardUI.SetConfirmed();

            if (heroAction.targetInfo.targetTeam != TargetTeam.NONE) {
                List<FightingEntity> possibleTargets = heroAction.GetAllPossibleActiveTargets(_field.GetHeroPlayer());
                switch (heroAction.targetInfo.targetType) {
                    case TargetType.SINGLE:
                        this._ui.targetSelectionUI.InitializeTargetSelectionSingle(possibleTargets, 0, this._commandSelector);
                        break;

                    case TargetType.ALL:
                        this._commandSelector.Initialize(0, 1, null); // Only one choice
                        this._ui.targetSelectionUI.InitializeTargetSelectionAll(possibleTargets);
                        break;

                    default:

                    break;
                }


                this._heroMenuActions.Push(new HeroMenuAction(MenuState.TARGET));
            } else {
                HeroActionConfirmed();
                GetHeroMenuAction().onBackCallback = this.OnActionChooseBackCallback;
                this._field.GetHeroPlayer().SetQueuedAction(heroAction, new List<int>());
                CheckExecuteTurn();
                this._heroMenuActions.Push(new HeroMenuAction(MenuState.WAITING));
            }
        }
    }

    private void SetHeroTarget() {
        HeroActionConfirmed();

        HeroMenuAction menuAction = GetHeroMenuAction();
        menuAction.targetIndex = this._commandSelector.GetChoice();
        menuAction.onBackCallback = this.OnTargetChooseBackCallback;

        HeroMenuAction moveMenuAction = GetPreviousHeroMenuAction();

        ActionBase heroAction = moveMenuAction.currentHeroChoices[moveMenuAction.targetIndex].action;
    
        switch (heroAction.targetInfo.targetType) {
            case TargetType.SINGLE:
                List<FightingEntity> possibleTargets = heroAction.GetAllPossibleActiveTargets(this._field.GetHeroPlayer());
                FightingEntity target = possibleTargets[menuAction.targetIndex];
                this._field.GetHeroPlayer().SetQueuedAction(heroAction, new List<int>{target.targetId}  );
                break;

            case TargetType.ALL:
                List<int> allEnemies = this._field.GetActiveEnemyObjects().Map((EnemyObject enemy) => enemy.targetId);
                this._field.GetHeroPlayer().SetQueuedAction( heroAction, allEnemies );
            break;

            default:
            break;
        }


        this._heroMenuActions.Push(new HeroMenuAction(MenuState.WAITING));
    }

    private void HeroActionConfirmed() {
        this._ui.targetSelectionUI.ClearSelection();
        this._ui.commandCardUI.SetCommandCardUI(CommandCardUIMode.CLOSED);
    }

    private bool CheckExecuteTurn() {
        if (_ui.tmp_TimerText == null) {
            return false;
        }

        List<PlayerObject> players = this._field.GetActivePlayerObjects();

        bool timeOutIfChatTooSlow = (this._field.GetHeroPlayer().HasSetCommand() && _playerActionCounter >= this.maxTimeBeforeAction);
        //bool timeOutIfChatTooSlow = false;
        bool startTurn = timeOutIfChatTooSlow || HasEveryoneEnteredActions();
        if (startTurn) {
            this._playerActionCounter = 0;

        } else {
            UpdateStateText();
        }
        return startTurn;
    }

    private void UpdateStateText() {
        // Currently not in the UI, but may add something similar later
        if (!this._field.GetHeroPlayer().HasSetCommand()) {
            this._ui.tmp_TimerText.text.SetText("Waiting on streamer input");
            this._ui.tmp_TimerText.gameObject.SetActive(true);
        } else {
            int timer = (int)(this.maxTimeBeforeAction - this._playerActionCounter);
            this._ui.tmp_TimerText.text.SetText("Waiting on Chat: " + timer);
        }
    }

    private bool HasEveryoneEnteredActions() {
        foreach (var player in this._field.GetActivePlayerObjects()) {
            if (player.HasSetCommand() == false) {
                return false;
            }
        }
        return true;
    }

    private void GoBackToLastHeroAction() {
        this._playerActionCounter = 0;
        this._heroMenuActions.Pop();
        HeroMenuAction menuAction = GetHeroMenuAction();
        if (menuAction.onBackCallback != null) {
            menuAction.onBackCallback();
        }
    }

    public HeroMenuAction GetPreviousHeroMenuAction() {
        HeroMenuAction top = GetHeroMenuAction();
        this._heroMenuActions.Pop();

        HeroMenuAction previousHeroAction = GetHeroMenuAction();
        this._heroMenuActions.Push(top);

        return previousHeroAction;
    }

    //
    // UI Callback
    //
    private void OnActionChooseBackCallback() {
        this._ui.targetSelectionUI.ClearSelection();
        InitializeCommandCardActionUI(GetHeroMenuAction().currentHeroChoices);
        this._field.GetHeroPlayer().ResetCommand();
    }

    private void OnTargetChooseBackCallback() {
        this._ui.targetIconsUI.ClearTargetsForFighter(this._field.GetHeroPlayer());
        this._field.GetHeroPlayer().ResetCommand();
        GoBackToLastHeroAction();
    }
}