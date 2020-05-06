using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattlePhase{
    PARTY_SETUP,
    MOVE_SELECTION,
    ENTER_BATTLE,
    START_TURN,
    END_TURN
};

public enum HeroInputActionState {
    SELECT_ACTION,
    SELECT_ENEMY_TARGET,
    SELECT_PLAYER_TARGET,
    BATTLE_START
}

[RequireComponent(typeof(BattleUI))]
[RequireComponent(typeof(StageInfo))]
[RequireComponent(typeof(BattleTurnController))]
public class BattleController : MonoBehaviour
{

    public BattleUI ui {get; set; }

    public StageInfo stage {get; set; }

    public BattleTurnController turn {get; set; }

    [Header ("References")]

    public CommandSelector commandSelector;


    [Header ("Other")]
    public float maxTimeBeforeAction = 15;
    public float playerActionCounter {get; set;}


    int heroActionIndex = 0;
    

    public bool inputActionsPhase{get; set; }


    public Text stateText;



    private HeroInputActionState heroInputActionState = HeroInputActionState.SELECT_ACTION;
    private int heroTargetIndex = 0;


    void Awake() {
        ui = GetComponent<BattleUI>();
        stage = GetComponent<StageInfo>();
        turn = GetComponent<BattleTurnController>();
    }

    void Start() {
        GameManager.Instance.battleController = this;

        this.Initialize();
    }

    private void Initialize() {
        this.stage.Initialize();
        this.ui.statusBarsUI.Initialize();
        this.OnPlayerTurnStart();
    }



    // Update is called once per frame
    void Update()
    {

        if (inputActionsPhase == true) {

            switch (heroInputActionState) {
                case HeroInputActionState.SELECT_ACTION:
                    if (Input.GetKeyDown(KeyCode.Z)) {
                       this.SetHeroAction();
                    }
                    break;

                case HeroInputActionState.SELECT_ENEMY_TARGET:
                    if (Input.GetKeyDown(KeyCode.Z)) {
                        this.SetHeroTarget();
                    }
                    break;

                default:

                    break;
            }

            playerActionCounter += Time.deltaTime;
            this.CheckExecuteTurn();
        } else {
            stateText.text = "";
        }
    }

    private void CheckExecuteTurn() {
        //bool timeOutIfChatTooSlow = (stage.GetHeroPlayer().HasSetCommand() && playerActionCounter >= this.maxTimeBeforeAction);
        bool timeOutIfChatTooSlow = false;
        bool startTurn = timeOutIfChatTooSlow || hasEveryoneEnteredActions();
        if (startTurn) {
            playerActionCounter = 0;
            this.ExecutePlayerTurn();
        } else {
            this.UpdateStateText();

        }
    }

    private void ExecutePlayerTurn() {
        this.SetUnsetMookCommands();

        this.inputActionsPhase = false;

        this.ui.commandCardUI.InitializeCommantaryUI();

        // Sort by player speed
        List<FightingEntity> orderedPlayers = new List<FightingEntity>(stage.GetAllFightingEntities());
        orderedPlayers.Sort( (FightingEntity a, FightingEntity b) =>  {  return b.stats.GetSpeed().CompareTo(a.stats.GetSpeed()); });

        this.DoActionHelper(orderedPlayers, 0);
        
    }

    private void UpdateStateText() {
        if (!stage.GetHeroPlayer().HasSetCommand()) {
            stateText.text = "Waiting on streamer input";
        } else {
            stateText.text = "Waiting on Chat: " + (int)(this.maxTimeBeforeAction - playerActionCounter);
        }
    }

    private void OnPlayerTurnStart() {
        this.inputActionsPhase = true;
        this.heroInputActionState = HeroInputActionState.SELECT_ACTION;

        this.InitializeCommandCardActionUI();


        this.playerActionCounter = 0;
        
        // Reset defence state
        foreach (var player in stage.GetPlayers()) {
            player.ResetCommand();

            if (player.modifiers.Contains("defend")) {
                player.modifiers.Remove("defend");
            }
        }
    }

    private void SetHeroAction() {
        this.heroInputActionState = HeroInputActionState.SELECT_ENEMY_TARGET;
        heroActionIndex = this.commandSelector.GetChoice();

        ActionBase heroAction = stage.GetHeroPlayer().actions[heroActionIndex];
    
        // TODO: Differentiate between ALL and single target.
        this.ui.targetSelectionUI.InitializeTargetSelectionSingle(heroAction.GetPotentialTargets(stage.GetHeroPlayer()), 0, this.commandSelector);
    }

    private void SetHeroTarget() {
        heroInputActionState = HeroInputActionState.BATTLE_START;
        this.heroTargetIndex = this.commandSelector.GetChoice();
        this.ui.targetSelectionUI.ClearSelection();
        stage.GetHeroPlayer().SetQueuedAction(new QueuedAction(stage.GetHeroPlayer(), stage.GetHeroPlayer().actions[heroActionIndex], new List<int>{heroTargetIndex}  ));
    }

    // Initializes the command card to display the hero's actions
    private void InitializeCommandCardActionUI() {
        List<string> actionNames = stage.GetHeroPlayer().actions.Map((ActionBase action) => { return action.name; });
        this.ui.commandCardUI.InitializeCommandSelection(actionNames, 0, this.commandSelector);
    }

    private bool hasEveryoneEnteredActions() {
        foreach (var player in stage.GetPlayers()) {
            if (player.HasSetCommand() == false) {
                return false;
            }
        }

        return true;
    }

    private void SetUnsetMookCommands() {
        foreach (var player in stage.GetPlayers()) {
            if (player.HasSetCommand() == false) {
                player.SetQueuedAction(new QueuedAction(player, player.actions[0], new List<int>{stage.GetRandomEnemyIndex()}  ));
            }
        }
    }

    private void DoActionHelper(List<FightingEntity> orderedEntities, int curIndex) {
        if (curIndex == orderedEntities.Count) {
            this.OnPlayerTurnStart();
            return;
        }
        
        FightingEntity entity = orderedEntities[curIndex];
        if (entity.GetType() == typeof(Enemy)) {
            entity.SetQueuedAction(new QueuedAction(entity, entity.GetRandomAction(), new List<int>{stage.GetRandomPlayerIndex()}  ));
        } else {
        }

        StartCoroutine(dummyAttackAnimation(entity, orderedEntities, curIndex));
    }

    private IEnumerator dummyAttackAnimation(FightingEntity a, List<FightingEntity> orderedEntities, int index) {

        string attackerName = a.Name;
        if (a.GetQueuedAction() == null) {
            Debug.LogError("Cannot find queued action: "  + attackerName);
            yield break;
        }
        string attackName = a.GetQueuedAction()._action.name;
        List<FightingEntity> targets = a.GetQueuedAction()._action.GetTargets(a, a.GetQueuedAction().GetTargetIds());

        string commentaryText;
        if (targets.Count == 1) {
            commentaryText = "" + attackerName + " used " + attackName + " on " + targets[0].Name;
        } else {
            commentaryText = "" + attackerName + " used " + attackName;
        }

        this.ui.commandCardUI.SetCommentaryUIText(commentaryText);
        
        yield return new WaitForSeconds(2f);
        this.DoAction(a);
        this.DoActionHelper(orderedEntities, index + 1);
    }


    private void DoAction(FightingEntity a) {
        a.GetQueuedAction().ExecuteAction();
        this.ui.statusBarsUI.UpdateStatusBarUI();
    }
}
