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
public class BattleController : MonoBehaviour
{
    [Header ("References")]

    public BattleUI ui;

    public StageInfo stage;

    public CommandSelector commandSelector;


    [Header ("Other")]
    public float maxTimeBeforeAction = 15;
    public float playerActionCounter {get; set;}


    
    public Color targetSelectionColor;



    int heroActionIndex = 0;
    

    public bool inputActionsPhase{get; set; }




    public Text stateText;



    private HeroInputActionState heroInputActionState = HeroInputActionState.SELECT_ACTION;
    private int heroTargetIndex = 0;


    void Start() {
        GameManager.Instance.battleController = this;

        // Instantiate hero

        // TODO: Waves
        this.stage.Initialize();
        this.ui.statusBarsUI.Initialize();
        this.OnPlayerTurnStart();
    }

    private void InitializeCommandCardActionUI() {
        List<string> actionNames = stage.GetHeroPlayer().actions.Map((ActionBase action) => { return action.name; });
        this.ui.commandCardUI.InitializeCommandSelection(actionNames, 0, this.commandSelector);
    }


    // Update is called once per frame
    void Update()
    {

        if (inputActionsPhase == true) {

            switch (heroInputActionState) {
                case HeroInputActionState.SELECT_ACTION:
                    if (Input.GetKeyDown(KeyCode.Z)) {
                        this.heroInputActionState = HeroInputActionState.SELECT_ENEMY_TARGET;
                        this.UpdateTargetSelectionUI();

                        this.commandSelector.Initialize(0, stage.GetEnemies().Count-1, this.UpdateTargetSelectionUI, true);
                    }
                    break;

                case HeroInputActionState.SELECT_ENEMY_TARGET:
                    if (Input.GetKeyDown(KeyCode.Z)) {
                        heroInputActionState = HeroInputActionState.BATTLE_START;
                        this.UpdateTargetSelectionUI();
                        stage.GetHeroPlayer().SetQueuedAction(new QueuedAction(stage.GetHeroPlayer(), stage.GetHeroPlayer().actions[heroActionIndex], new List<int>{heroTargetIndex}  ));
                    }
                    break;

                default:

                    break;
            }

            playerActionCounter += Time.deltaTime;
            //bool timeOutIfChatTooSlow = (stage.GetHeroPlayer().HasSetCommand() && playerActionCounter >= this.maxTimeBeforeAction);
            bool timeOutIfChatTooSlow = false;

            bool startTurn = timeOutIfChatTooSlow || hasEveryoneEnteredActions();
            if (startTurn) {
                playerActionCounter = 0;
                this.ExecutePlayerTurn();
            } else if (!stage.GetHeroPlayer().HasSetCommand()) {
                    stateText.text = "Waiting on streamer input";
            } else {
                stateText.text = "Waiting on Chat: " + (int)(this.maxTimeBeforeAction - playerActionCounter);
            }
        
        } else {
            stateText.text = "";
        }
    }


    private bool hasEveryoneEnteredActions() {
        foreach (var player in stage.GetPlayers()) {
            if (player.HasSetCommand() == false) {
                return false;
            }
        }

        return true;
    }


    public void UpdateTargetSelectionUI() {

        this.heroTargetIndex = this.commandSelector.GetChoice();

        //TODO: Update this animation
        switch (heroInputActionState) {
            case HeroInputActionState.SELECT_ENEMY_TARGET:
                foreach (var enemy in stage.GetEnemies()) {
                    enemy.GetComponent<SpriteRenderer>().color = Color.white;
                }
                stage.GetEnemies()[heroTargetIndex].GetComponent<SpriteRenderer>().color = targetSelectionColor;
            break;

            case HeroInputActionState.SELECT_PLAYER_TARGET:
                // TODO: 
                break;

            default:
                foreach (var enemy in stage.GetEnemies()) {
                    enemy.GetComponent<SpriteRenderer>().color = Color.white;
                }
                break;
        }
    }


    public void ExecutePlayerTurn() {
        this.SetUnsetMookCommands();

        this.inputActionsPhase = false;

        this.ui.commandCardUI.InitializeCommantaryUI();

        // Sort by player speed
        List<FightingEntity> orderedPlayers = new List<FightingEntity>(stage.GetAllFightingEntities());
        orderedPlayers.Sort( (FightingEntity a, FightingEntity b) =>  {  return b.stats.GetSpeed().CompareTo(a.stats.GetSpeed()); });

        this.DoActionHelper(orderedPlayers, 0);
        
    }

    private void SetUnsetMookCommands() {
        foreach (var player in stage.GetPlayers()) {
            if (player.HasSetCommand() == false) {
                player.SetQueuedAction(new QueuedAction(player, player.actions[0], new List<int>{GetRandomEnemyIndex()}  ));
            }
        }
    }

    private int GetRandomEnemyIndex() {
        List<int> validIndices = new List<int>();
        for (int i = 0; i < stage.GetEnemies().Count; i++) {
            if (stage.GetEnemies()[i] != null) {
                validIndices.Add(i);
            }
        }

        return validIndices[Random.Range(0, validIndices.Count)];
    }


    public void DoActionHelper(List<FightingEntity> orderedEntities, int curIndex) {
        if (curIndex == orderedEntities.Count) {
            this.OnPlayerTurnStart();
            return;
        }
        
        FightingEntity entity = orderedEntities[curIndex];
        if (entity.GetType() == typeof(Enemy)) {
            entity.SetQueuedAction(new QueuedAction(entity, entity.GetRandomAction(), new List<int>{GetRandomPlayerTarget()}  ));
        } else {
        }

        StartCoroutine(dummyAttackAnimation(entity, orderedEntities, curIndex));
    }

    public int GetRandomPlayerTarget() {
        int enemyTarget = Random.Range(0, stage.GetPlayers().Count);
        return enemyTarget;
    }

    IEnumerator dummyAttackAnimation(FightingEntity a, List<FightingEntity> orderedEntities, int index) {

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

    public void OnPlayerTurnStart() {
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

    public void DoAction(FightingEntity a) {
        a.GetQueuedAction().ExecuteAction();
        this.ui.statusBarsUI.UpdateStatusBarUI();
    }
}
