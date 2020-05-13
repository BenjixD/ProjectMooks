using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattlePhase{
    TURN_START,
    PARTY_SETUP,
    MOVE_SELECTION,
    MAGIC_SELECTION,
    TARGET_SELECTION,
    WAITING_FOR_CHAT,
    BATTLE
};

[System.Serializable]
public class HeroActionChoice {
    public string choiceName = "";
    public ActionBase action = null;

    public HeroActionChoice(string actionName, ActionBase action) {
        this.choiceName = actionName;
        this.action = action;
    }
}


[RequireComponent(typeof(BattleUI))]
[RequireComponent(typeof(FieldState))]
public class TurnController : MonoBehaviour
{

    public BattleUI ui {get; set; }

    public FieldState field {get; set; }
    public Battle battle {get; set; }


    [Header ("References")]

    public CommandSelector commandSelector;


    [Header ("Other")]
    public float maxTimeBeforeAction = 15;
    public BattlePhase battlePhase = BattlePhase.PARTY_SETUP;

    private float playerActionCounter {get; set;}

    public List<HeroActionChoice> currentHeroChoices;

    // Selection
    private int _heroActionIndex = 0;
    private int _heroTargetIndex = 0;


    void Awake() {
        ui = GetComponent<BattleUI>();
        field = GetComponent<FieldState>();
    }

    void Start() {
        GameManager.Instance.turnController = this;
        this.Initialize();
    }

    private void Initialize() {
        Messenger.AddListener(Messages.OnTurnStart, this.onTurnStart);
        Messenger.AddListener(Messages.OnTurnEnd, this.onTurnEnd);
        Messenger.AddListener(Messages.OnPartySetup, this.onPartySetup);
        Messenger.AddListener(Messages.OnMoveSelection, this.onMoveSelection);

        Messenger.AddListener<BattleResult>(Messages.OnBattleEnd, this.onBattleEnd);

        this.field.Initialize();
        this.ui.statusBarsUI.Initialize();

        this.BroadcastOnStartTurn();
    }

    public void BroadcastPartySetup() {
        Messenger.Broadcast(Messages.OnPartySetup);
    }

    public void BroadcastMoveSelection() {
        Messenger.Broadcast(Messages.OnMoveSelection);
    }    


    public void BroadcastOnStartTurn() {
        Messenger.Broadcast(Messages.OnTurnStart);
    }

    public void BroadcastOnTurnEnd() {
        Messenger.Broadcast(Messages.OnTurnEnd);
    }

    public bool CanInputActions() {
        return this.battlePhase != BattlePhase.BATTLE;
    }

    void Update()
    {   
        if (this.CanInputActions()) {

            switch (battlePhase) {
                case BattlePhase.MOVE_SELECTION:
                    if (Input.GetKeyDown(KeyCode.Z)) {
                        this.setHeroAction();
                    }
                    break;

                case BattlePhase.TARGET_SELECTION:
                    if (Input.GetKeyDown(KeyCode.Z)) {
                        this.setHeroTarget();
                    }
                    break;
                case BattlePhase.MAGIC_SELECTION:
                    if (Input.GetKeyDown(KeyCode.Z)) {
                        this.setHeroAction();
                    }
                    break;
                default:
                    break;
            }

            playerActionCounter += Time.deltaTime;
            this.checkExecuteTurn();
        }
    }



    private void onTurnStart() {

        this.battlePhase = BattlePhase.TURN_START;
        this.battle = null;

        this.initializeCommandCardActionUI(ActionType.BASIC);


        this.playerActionCounter = 0;
        
        // Reset defence state
        foreach (var player in field.GetAllFightingEntities()) {
            player.ResetCommand();

            // TODO: Make defense a status buff
            if (player.modifiers.Contains("defend")) {
                player.modifiers.Remove("defend");
            }
        }

        this.ApplyStatusAilments(this.battlePhase);
        this.BroadcastPartySetup();
    }


    private void onPartySetup() {
        this.battlePhase = BattlePhase.PARTY_SETUP;
        this.field.RequestRecruitNewParty();
        this.BroadcastMoveSelection();
    }

    private void onMoveSelection() {
        this.battlePhase = BattlePhase.MOVE_SELECTION;
        this.ApplyStatusAilments(this.battlePhase);
    }

    private void onBattleEnd(BattleResult result) {
        this.BroadcastOnTurnEnd();
    }

    private void onTurnEnd() {
        this.ApplyStatusAilments(this.battlePhase);
        this.DecrementStatusAilmentDuration();
        this.BroadcastOnStartTurn();
    }

    private void checkExecuteTurn() {
        List<Player> players = field.GetActivePlayers();

        //bool timeOutIfChatTooSlow = (stage.GetHeroPlayer().HasSetCommand() && playerActionCounter >= this.maxTimeBeforeAction);
        bool timeOutIfChatTooSlow = false;
        bool startTurn = timeOutIfChatTooSlow || hasEveryoneEnteredActions();
        if (startTurn) {
            playerActionCounter = 0;
            this.ui.SetStateText("");
            this.executePlayerTurn();
        } else {
            this.UpdateStateText();

        }
    }

    private void executePlayerTurn() {
        this.battlePhase = BattlePhase.BATTLE;
        this.battle = new Battle(this);
        this.battle.StartBattle();
    }

    private void UpdateStateText() {
        if (!field.GetHeroPlayer().HasSetCommand()) {
            this.ui.SetStateText("Waiting on streamer input");
        } else {
            int timer = (int)(this.maxTimeBeforeAction - playerActionCounter);
            this.ui.SetStateText("Waiting on Chat: " + timer);
        }
    }

    private void setHeroAction() {
        _heroActionIndex = this.commandSelector.GetChoice();

        HeroActionChoice choice = this.currentHeroChoices[_heroActionIndex];

        if (choice.choiceName == "Magic" && choice.action == null) {
            this.battlePhase = BattlePhase.MAGIC_SELECTION;
            this.initializeCommandCardActionUI(ActionType.MAGIC);
        } else {
            ActionBase heroAction = choice.action;
            // TODO: Differentiate between ALL and single target.
            if (heroAction.targetInfo.targetTeam != TargetTeam.NONE) {
                List<FightingEntity> possibleTargets = heroAction.GetPotentialActiveTargets(field.GetHeroPlayer());
                switch (heroAction.targetInfo.targetType) {
                    case TargetType.SINGLE:
                        this.ui.targetSelectionUI.InitializeTargetSelectionSingle(possibleTargets, 0, this.commandSelector);
                        break;

                    case TargetType.ALL:
                        this.ui.targetSelectionUI.InitializeTargetSelectionAll(possibleTargets);
                    break;

                    default:

                    break;
                }
                this.battlePhase = BattlePhase.TARGET_SELECTION;
            } else {
                field.GetHeroPlayer().SetQueuedAction(new QueuedAction(field.GetHeroPlayer(), heroAction, new List<int>()));
                this.checkExecuteTurn();
            }
        }
    }

    private void setHeroTarget() {
        this.battlePhase = BattlePhase.WAITING_FOR_CHAT;
        this._heroTargetIndex = this.commandSelector.GetChoice();
        this.ui.targetSelectionUI.ClearSelection();
        ActionBase heroAction = this.currentHeroChoices[_heroActionIndex].action;
    

        switch (heroAction.targetInfo.targetType) {
            case TargetType.SINGLE:
                List<FightingEntity> possibleTargets = heroAction.GetPotentialActiveTargets(field.GetHeroPlayer());
                FightingEntity target = possibleTargets[this._heroTargetIndex];
                field.GetHeroPlayer().SetQueuedAction(new QueuedAction(field.GetHeroPlayer(), heroAction, new List<int>{target.targetId}  ));
                break;

            case TargetType.ALL:
                List<int> allEnemies = field.GetActiveEnemies().Map((Enemy enemy) => enemy.targetId);
                field.GetHeroPlayer().SetQueuedAction(new QueuedAction(field.GetHeroPlayer(), heroAction, allEnemies ));
            break;

            default:

            break;
        }
    }

    // Initializes the command card to display the hero's actions
    private void initializeCommandCardActionUI(ActionType type) {
        Debug.Log("Initialize command card: " + type);
        FightingEntity heroPlayer = field.GetHeroPlayer();
        List<ActionBase> actions = field.GetHeroPlayer().GetFilteredActions(type);
        this.currentHeroChoices = new List<HeroActionChoice>();

        foreach (ActionBase action in actions) {
            this.currentHeroChoices.Add(new HeroActionChoice(action.name, action));
        }

        if (type == ActionType.BASIC) {
            if (field.GetHeroPlayer().GetFilteredActions(ActionType.MAGIC).Count != 0) {
                this.currentHeroChoices.Insert(1, new HeroActionChoice("Magic", null));
            }
        }

        List<string> actionNames = this.currentHeroChoices.Map((HeroActionChoice choice) => { return choice.choiceName; });
        this.ui.commandCardUI.InitializeCommandSelection(actionNames, 0, this.commandSelector);
    }

    private bool hasEveryoneEnteredActions() {
        foreach (var player in field.GetActivePlayers()) {
            if (player.HasSetCommand() == false) {
                return false;
            }
        }

        return true;
    }

    // Triggers status ailments
    private void ApplyStatusAilments(BattlePhase bp) {
        foreach (var entity in field.GetAllFightingEntities()) {
            entity.GetAilmentController().TickAilmentEffects(bp);
        }
    }

    private void DecrementStatusAilmentDuration() {
        foreach(var entity in field.GetAllFightingEntities()) {
            entity.GetAilmentController().DecrementAllAilmentsDuration();
        }
    }

    void OnDestroy() {
        Messenger.RemoveListener(Messages.OnTurnStart, this.onTurnStart);
        Messenger.RemoveListener(Messages.OnTurnEnd, this.onTurnEnd);
        Messenger.RemoveListener(Messages.OnPartySetup, this.onPartySetup);
        Messenger.RemoveListener(Messages.OnMoveSelection, this.onMoveSelection);
        Messenger.RemoveListener<BattleResult>(Messages.OnBattleEnd, this.onBattleEnd);
    }

}
