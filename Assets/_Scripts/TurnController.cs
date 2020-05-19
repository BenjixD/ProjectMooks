using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum BattlePhase{
    TURN_START,
    PARTY_SETUP,
    MOVE_SELECTION,
    BATTLE,
    TURN_END
};

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

// The main controller/manager for a battle
[RequireComponent(typeof(BattleUI))]
[RequireComponent(typeof(BattleField))]
[RequireComponent(typeof(StageController))]
public class TurnController : MonoBehaviour
{

    public BattleUI ui {get; set; }

    public BattleField field {get; set; }
    public Battle battle {get; set; }
    public StageController stageController {get; set;}


    [Header ("References")]

    public CommandSelector commandSelector;


    [Header ("Other")]
    public float maxTimeBeforeAction = 15;
    public BattlePhase battlePhase = BattlePhase.PARTY_SETUP;
    public MenuState menuState = MenuState.MOVE;

    private float playerActionCounter {get; set;}

    public List<HeroActionChoice> currentHeroChoices;

    // Selection
    private int _heroActionIndex = 0;
    private int _heroTargetIndex = 0;


    void Awake() {
        ui = GetComponent<BattleUI>();
        field = GetComponent<BattleField>();
        stageController = GetComponent<StageController>();
    }

    void Start() {
        GameManager.Instance.turnController = this;
        this.Initialize();
    }

    private void Initialize() {
        this.AddListeners();

        this.field.Initialize();
        this.ui.statusBarsUI.Initialize();

        this.BroadcastOnStartTurn();
    }

    void OnDestroy() {
        this.RemoveListeners();
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

    public void BroadcastOnBattleStart() {
        Messenger.Broadcast(Messages.OnBattleStart);
    }

    public void BroadcastOnBattleEnd(BattleResult result) {
        Messenger.Broadcast<BattleResult>(Messages.OnBattleEnd, result);
    }

    public void BroadcastOnTurnEnd() {
        Messenger.Broadcast(Messages.OnTurnEnd);
    }

    public bool CanInputActions() {
        return this.battlePhase == BattlePhase.MOVE_SELECTION;
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
        Debug.Log("Starting move selection");
        StartCoroutine(HeroMoveSelection());
    }

    private void onBattleStart() {
        this.battlePhase = BattlePhase.BATTLE;
        this.battle = new Battle(this);
        this.battle.StartBattle();
    }

    private void onBattleEnd(BattleResult result) {
        this.BroadcastOnTurnEnd();
    }

    private void onTurnEnd() {
        this.battlePhase = BattlePhase.TURN_END;
        this.ApplyStatusAilments(this.battlePhase);
        this.DecrementStatusAilmentDuration();
        this.BroadcastOnStartTurn();
    }

    private bool checkExecuteTurn() {
        List<Player> players = field.playerParty.GetActiveMembers();

        //bool timeOutIfChatTooSlow = (stage.GetHeroPlayer().HasSetCommand() && playerActionCounter >= this.maxTimeBeforeAction);
        bool timeOutIfChatTooSlow = false;
        bool startTurn = timeOutIfChatTooSlow || hasEveryoneEnteredActions();
        if (startTurn) {
            playerActionCounter = 0;
        } else {
            this.UpdateStateText();
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

    private void setHeroAction() {
        _heroActionIndex = this.commandSelector.GetChoice();
        this.ui.commandCardUI.SetConfirmed();

        HeroActionChoice choice = this.currentHeroChoices[_heroActionIndex];

        if (choice.choiceName == "Magic" && choice.action == null) {
            this.menuState = MenuState.MAGIC;
            this.initializeCommandCardActionUI(ActionType.MAGIC);
        } else {
            ActionBase heroAction = choice.action;
            if (heroAction.targetInfo.targetTeam != TargetTeam.NONE) {
                List<FightingEntity> possibleTargets = heroAction.GetAllPossibleActiveTargets(field.GetHeroPlayer());
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
                this.menuState = MenuState.TARGET;
            } else {
                field.GetHeroPlayer().SetQueuedAction(new QueuedAction(field.GetHeroPlayer(), heroAction, new List<int>()));
                this.checkExecuteTurn();
            }
        }
    }

    private void setHeroTarget() {
        this.menuState = MenuState.WAITING;
        this._heroTargetIndex = this.commandSelector.GetChoice();
        this.ui.targetSelectionUI.ClearSelection();
        this.ui.commandCardUI.SetCommandCardUI(CommandCardUIMode.CLOSED);
        ActionBase heroAction = this.currentHeroChoices[_heroActionIndex].action;
    

        switch (heroAction.targetInfo.targetType) {
            case TargetType.SINGLE:
                List<FightingEntity> possibleTargets = heroAction.GetAllPossibleActiveTargets(field.GetHeroPlayer());
                FightingEntity target = possibleTargets[this._heroTargetIndex];
                field.GetHeroPlayer().SetQueuedAction(new QueuedAction(field.GetHeroPlayer(), heroAction, new List<int>{target.targetId}  ));
                break;

            case TargetType.ALL:
                List<int> allEnemies = field.enemyParty.GetActiveMembers().Map((Enemy enemy) => enemy.targetId);
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
        foreach (var player in field.playerParty.GetActiveMembers()) {
            if (player.HasSetCommand() == false) {
                return false;
            }
        }

        return true;
    }

    private void onWaveComplete() {
        StageInfoContainer stageInfo = GameManager.Instance.gameState.GetCurrentStage();
        WaveInfoContainer waveInfo = stageInfo.GetWaveInfo(this.field.currentWaveIndex);

        this.field.currentWaveIndex++;
        if (this.field.currentWaveIndex >= stageInfo.numWaves) {
            this.stageController.LoadNextStage();
        } else {
            this.field.GenerateEnemyList(this.field.currentWaveIndex);
        }
    }

    private void onEntityDeath(DeathResult result) {
        FightingEntity deadFighter = result.deadEntity.fighter;
        // TODO: Play death animation

        bool isEnemy = deadFighter.isEnemy();
        if (isEnemy) {
            this.field.enemyParty.members[deadFighter.targetId] = null;
            Destroy(deadFighter.gameObject);

            bool stillHasEnemies = false;
            for (int i = 0; i < this.field.enemyParty.members.Length; i++) {
                if (this.field.enemyParty.members[i] != null) {
                    stillHasEnemies = true;
                    break;
                }
            }

            if (!stillHasEnemies) {
                this.onWaveComplete();
            }
        } else {

            if (deadFighter.targetId == 0) {
                this.onHeroDeath(result);
                return;
            }

            this.field.playerParty.members[deadFighter.targetId] = null;
            GameManager.Instance.party.EvictPlayer(deadFighter.targetId);
            Destroy(deadFighter.gameObject);
        }

        this.ui.statusBarsUI.UpdateStatusBars();
    }

    private void onHeroDeath(DeathResult result) {
        this.stageController.LoadDeathScene();
    }


    private void AddListeners() {
        Messenger.AddListener<DeathResult>(Messages.OnEntityDeath, this.onEntityDeath);
        Messenger.AddListener(Messages.OnWaveComplete, this.onWaveComplete);

        Messenger.AddListener(Messages.OnTurnStart, this.onTurnStart);
        Messenger.AddListener(Messages.OnTurnEnd, this.onTurnEnd);
        Messenger.AddListener(Messages.OnPartySetup, this.onPartySetup);
        Messenger.AddListener(Messages.OnMoveSelection, this.onMoveSelection);
        Messenger.AddListener(Messages.OnBattleStart, this.onBattleStart);

        Messenger.AddListener<BattleResult>(Messages.OnBattleEnd, this.onBattleEnd);
    }

    private void RemoveListeners() {
        Messenger.RemoveListener<DeathResult>(Messages.OnEntityDeath, this.onEntityDeath);
        Messenger.RemoveListener(Messages.OnWaveComplete, this.onWaveComplete);

        Messenger.RemoveListener(Messages.OnTurnStart, this.onTurnStart);
        Messenger.RemoveListener(Messages.OnTurnEnd, this.onTurnEnd);
        Messenger.RemoveListener(Messages.OnPartySetup, this.onPartySetup);
        Messenger.RemoveListener(Messages.OnMoveSelection, this.onMoveSelection);
        Messenger.RemoveListener(Messages.OnBattleStart, this.onBattleStart);

        Messenger.RemoveListener<BattleResult>(Messages.OnBattleEnd, this.onBattleEnd);
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

    // Coroutines
    IEnumerator HeroMoveSelection() {
        menuState = MenuState.MOVE;
        while(true) {
            switch (menuState) {
                case MenuState.MOVE:
                    if (Input.GetKeyDown(KeyCode.Z)) {
                        this.setHeroAction();
                    }
                    break;

                case MenuState.TARGET:
                    if (Input.GetKeyDown(KeyCode.Z)) {
                        this.setHeroTarget();
                    }
                    break;
                case MenuState.MAGIC:
                    if (Input.GetKeyDown(KeyCode.Z)) {
                        this.setHeroAction();
                    }
                    break;
                default:
                    break;
            }

            playerActionCounter += Time.deltaTime;
            if(this.checkExecuteTurn()) {
                this.BroadcastOnBattleStart();
                break;
            }
            yield return null;
        }
    }
}
