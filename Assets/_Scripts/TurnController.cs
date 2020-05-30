using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum BattlePhase{
    TURN_START,
    PARTY_SETUP,
    MOVE_SELECTION,
    BATTLE,
    TURN_END,
    NONE
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

public class HeroMenuAction {

    public MenuState menuState = MenuState.MOVE;
    public int targetIndex;
    public List<HeroActionChoice> currentHeroChoices;

    public UnityAction onBackCallback = null;

    public HeroMenuAction(MenuState state) {
        this.menuState = state;
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

    private float playerActionCounter {get; set;}


    // Selection
    Stack<HeroMenuAction> heroMenuActions = new Stack<HeroMenuAction>();


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
        this.ui.Initialize();

        this.BroadcastOnStartTurn();
    }

    void OnDestroy() {
        this.RemoveListeners();
    }

    public void BroadcastPartySetup() {
        Messenger.Broadcast(Messages.OnPartySetup);
        this.ui.playerQueueUI.RefreshUI();
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
        this.heroMenuActions.Clear();

        this.playerActionCounter = 0;
        
        // Reset defence state
        foreach (var player in field.GetAllFightingEntities()) {
            player.ResetCommand();
        }

        this.ApplyStatusAilments(this.battlePhase);
        this.BroadcastPartySetup();
    }


    private void onPartySetup() {
        this.battlePhase = BattlePhase.PARTY_SETUP;
        PlayerObject heroPlayer = this.field.GetHeroPlayer();
        if (!heroPlayer.HasModifier(ModifierAilment.MODIFIER_DEATH)) {
            this.field.RequestRecruitNewParty();
        }

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
        List<PlayerObject> players = field.GetActivePlayerObjects();

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
        HeroMenuAction menuAction = this.GetHeroMenuAction();
        menuAction.targetIndex = this.commandSelector.GetChoice();
        this.ui.commandCardUI.SetConfirmed();
        menuAction.onBackCallback = this.OnActionChooseBackCallback;

        HeroActionChoice choice = menuAction.currentHeroChoices[menuAction.targetIndex];

        if (choice.choiceName == "Magic" && choice.action == null) {
            this.heroMenuActions.Push(new HeroMenuAction(MenuState.MAGIC));
            this.initializeCommandCardActionUI(this.getHeroActionChoices(ActionType.MAGIC));
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


                this.heroMenuActions.Push(new HeroMenuAction(MenuState.TARGET));
            } else {
                this.HeroActionConfirmed();
                this.GetHeroMenuAction().onBackCallback = this.OnActionChooseBackCallback;
                field.GetHeroPlayer().SetQueuedAction(new QueuedAction(field.GetHeroPlayer(), heroAction, new List<int>()));
                this.checkExecuteTurn();
                this.heroMenuActions.Push(new HeroMenuAction(MenuState.WAITING));
            }
        }
    }

    private void HeroActionConfirmed() {
        this.ui.targetSelectionUI.ClearSelection();
        this.ui.commandCardUI.SetCommandCardUI(CommandCardUIMode.CLOSED);
    }

    private void setHeroTarget() {
        this.HeroActionConfirmed();

        HeroMenuAction menuAction = this.GetHeroMenuAction();
        menuAction.targetIndex = this.commandSelector.GetChoice();
        menuAction.onBackCallback = this.OnTargetChooseBackCallback;

        HeroMenuAction moveMenuAction = this.GetPreviousHeroMenuAction();


        ActionBase heroAction = moveMenuAction.currentHeroChoices[moveMenuAction.targetIndex].action;
    
        switch (heroAction.targetInfo.targetType) {
            case TargetType.SINGLE:
                List<FightingEntity> possibleTargets = heroAction.GetAllPossibleActiveTargets(field.GetHeroPlayer());
                FightingEntity target = possibleTargets[menuAction.targetIndex];
                field.GetHeroPlayer().SetQueuedAction(new QueuedAction(field.GetHeroPlayer(), heroAction, new List<int>{target.targetId}  ));
                break;

            case TargetType.ALL:
                List<int> allEnemies = field.GetActiveEnemyObjects().Map((EnemyObject enemy) => enemy.targetId);
                field.GetHeroPlayer().SetQueuedAction(new QueuedAction(field.GetHeroPlayer(), heroAction, allEnemies ));
            break;

            default:

            break;
        }


        this.heroMenuActions.Push(new HeroMenuAction(MenuState.WAITING));
    }


    private List<HeroActionChoice> getHeroActionChoices(ActionType type) {
        FightingEntity heroPlayer = field.GetHeroPlayer();
        List<ActionBase> actions = field.GetHeroPlayer().GetFilteredActions(type);
        List<HeroActionChoice> currentHeroChoices = new List<HeroActionChoice>();

        foreach (ActionBase action in actions) {
            currentHeroChoices.Add(new HeroActionChoice(action.name, action));
        }

        if (type == ActionType.BASIC) {
            if (field.GetHeroPlayer().GetFilteredActions(ActionType.MAGIC).Count != 0) {
                currentHeroChoices.Insert(1, new HeroActionChoice("Magic", null));
            }
        }

        HeroMenuAction menuAction = this.GetHeroMenuAction();
        menuAction.targetIndex = 0;
        menuAction.currentHeroChoices = currentHeroChoices;

        return currentHeroChoices;
    }

    

    // Initializes the command card to display the hero's actions
    private void initializeCommandCardActionUI(List<HeroActionChoice> currentHeroChoices) {
        if (!CanDoHeroAction()) {
            return;
        }

        List<string> actionNames = currentHeroChoices.Map((HeroActionChoice choice) => { return choice.choiceName; });
        this.ui.commandCardUI.InitializeCommandSelection(actionNames, 0, this.commandSelector);
    }

    private bool hasEveryoneEnteredActions() {
        List<PlayerObject> activeMembers = field.GetActivePlayerObjects();
        foreach (var player in activeMembers) {
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
            GameManager.Instance.gameState.enemyParty.SetFighter(deadFighter.targetId, null);
            Destroy(deadFighter.gameObject);

            bool stillHasEnemies = false;
            EnemyObject[] enemies = this.field.GetEnemyObjects();
            for (int i = 0; i < enemies.Length; i++) {
                if (enemies[i] != null) {
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
            } else {
                this.EvictMook(deadFighter);
            }
        }

        this.ui.statusBarsUI.UpdateStatusBars();


         // Note: How we determine this part may change depending on how we do Mook deaths:
        List<PlayerObject> allPlayers = this.field.GetActivePlayerObjects();
        if (allPlayers.Count == 1 && this.field.GetHeroPlayer().HasModifier(ModifierAilment.MODIFIER_DEATH)) {
            this.stageController.LoadDeathScene();
        }
    }

    public void EvictMook(FightingEntity mookObject) {
        GameManager.Instance.gameState.playerParty.EvictPlayer(mookObject.targetId);
        Destroy(mookObject.gameObject);
        this.ui.statusBarsUI.UpdateStatusBars();
    }

    private void onHeroDeath(DeathResult result) {
        List<PlayerObject> allPlayers = this.field.GetActivePlayerObjects();

        PlayerObject heroPlayer = this.field.GetHeroPlayer();

        heroPlayer.DoDeathAnimation();

        StatusAilment reviveStatusAilmentPrefab = GameManager.Instance.models.GetCommonStatusAilment("Hero's Miracle");
        heroPlayer.GetAilmentController().AddStatusAilment(Instantiate(reviveStatusAilmentPrefab));
    
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

    public HeroMenuAction GetHeroMenuAction() {
        return this.heroMenuActions.Peek();
    }

    public HeroMenuAction GetPreviousHeroMenuAction() {
        HeroMenuAction top = GetHeroMenuAction();
        this.heroMenuActions.Pop();

        HeroMenuAction previousHeroAction = this.heroMenuActions.Peek();
        this.heroMenuActions.Push(top);

        return previousHeroAction;
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

    private void OnActionChooseBackCallback() {
        this.ui.targetSelectionUI.ClearSelection();
        this.initializeCommandCardActionUI(this.GetHeroMenuAction().currentHeroChoices);
    }

    private void OnTargetChooseBackCallback() {
        this.ui.targetIconsUI.ClearTargetsForFighter(this.field.GetHeroPlayer());
        this.goBackToLastHeroAction();
    }

    private bool CanDoHeroAction() {
        PlayerObject heroPlayer = this.field.GetHeroPlayer();
        return heroPlayer != null && !heroPlayer.HasModifier(ModifierAilment.MODIFIER_CANNOT_USE_ACTION);
    }

    // Coroutines
    IEnumerator HeroMoveSelection() {
        PlayerObject heroPlayer = this.field.GetHeroPlayer();

        if (heroPlayer != null) {
            this.heroMenuActions.Push(new HeroMenuAction(MenuState.MOVE));
            this.initializeCommandCardActionUI(this.getHeroActionChoices(ActionType.BASIC));
        }


        while(true) {
            if (CanDoHeroAction()) {
                HeroMenuAction menuAction = this.GetHeroMenuAction();
                MenuState menuState = menuAction.menuState;
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

                if (heroMenuActions.Count > 1) {
                    // Go back a menu
                    if (Input.GetKeyDown(KeyCode.X)) {
                        this.goBackToLastHeroAction();
                    }            
                }
            }

            playerActionCounter += Time.deltaTime;
            if(this.checkExecuteTurn()) {
                this.BroadcastOnBattleStart();
                break;
            }
            yield return null;
        }
    }

    private void goBackToLastHeroAction() {
        this.heroMenuActions.Pop();
        HeroMenuAction menuAction = this.GetHeroMenuAction();
        if (menuAction.onBackCallback != null) {
            menuAction.onBackCallback();
        }
    }
}
