using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum BattlePhase {
    PLAYER,
    ENEMY

};

public enum BattleOption {
    ATTACK = 0,
    MAGIC = 1,
    DEFEND = 2,
    LENGTH = 3
};


public class BattleController : MonoBehaviour
{

    public CommandOptionText commandTextPrefab;
    public RectTransform selectionCursor;
    public Vector2 selectionCursorOffset = new Vector2(0, 0);

    public float maxTimeBeforeAction = 15;
    public float playerActionCounter {get; set;}

    public RectTransform ActionMenu;

    public RectTransform ActionMenuTextParent;

    public RectTransform CommentaryMenu;
    public Text CommentaryText;

    Player _heroPlayer;
    int heroActionIndex = 0;
    public List<RectTransform> battleOptionsUI {get ; set; }

    bool startActions = false;


    void Start() {
        _heroPlayer = GetPlayers()[0];

        // TODO: Waves
        GameManager.Instance.GenerateEnemyList();
        battleOptionsUI = new List<RectTransform>();

        foreach (ActionBase action in _heroPlayer.actions) {
            CommandOptionText  commandText = Instantiate(commandTextPrefab) as CommandOptionText;
            commandText.text.text = action.name;
            commandText.transform.parent = ActionMenuTextParent.transform;
            battleOptionsUI.Add(commandText.GetComponent<RectTransform>());
        }

        this.OnPlayerTurnStart();
        this.UpdateBattleOptionUI();
    }

    private List<Player> GetPlayers() {
        return GameManager.Instance.party.GetPlayersInPosition();
    }

    // Update is called once per frame
    void Update()
    {

        if (startActions == false) {
            if (_heroPlayer.HasSetCommand() == false) {
                if (Input.GetKeyDown(KeyCode.DownArrow)) {
                    if (heroActionIndex == 0) {
                        heroActionIndex = _heroPlayer.actions.Count - 1;
                    } else {
                        heroActionIndex--;
                    }

                    this.UpdateBattleOptionUI();
                }

                if (Input.GetKeyDown(KeyCode.UpArrow)) {
                    if (heroActionIndex == _heroPlayer.actions.Count - 1) {
                        heroActionIndex = 0;
                    } else {
                        heroActionIndex++;
                    }

                    this.UpdateBattleOptionUI();
                }


                if (Input.GetKeyDown(KeyCode.Z)) {

                    _heroPlayer.SetQueuedAction(new QueuedAction(_heroPlayer, _heroPlayer.actions[heroActionIndex], new List<int>{0}, TargetType.ENEMY  ));

                }
            } else {
                
                if (Input.GetKeyDown(KeyCode.Z)) {

                }
            }



            playerActionCounter += Time.deltaTime;

            if ( (_heroPlayer.HasSetCommand() && playerActionCounter >= this.maxTimeBeforeAction) || hasEveryoneEnteredActions()) {
                playerActionCounter = 0;
                this.ExecutePlayerTurn();
            }
        }
    }

    private bool hasEveryoneEnteredActions() {
        foreach (var player in GetPlayers()) {
            if (player.HasSetCommand() == false) {
                return false;
            }
        }

        return true;
    }


    public void UpdateBattleOptionUI() {
        selectionCursor.transform.parent = battleOptionsUI[heroActionIndex].transform;
        selectionCursor.anchoredPosition = selectionCursorOffset;
    }


    public void ExecutePlayerTurn() {
        this.startActions = true;
        this.ActionMenu.gameObject.SetActive(false);
        this.CommentaryMenu.gameObject.SetActive(true);

        // Sort by player speed
        List<FightingEntity> orderedPlayers = new List<FightingEntity>(GameManager.Instance.getAllFightingEntities());
        orderedPlayers.Sort( (FightingEntity a, FightingEntity b) =>  {  return b.stats.GetSpeed().CompareTo(a.stats.GetSpeed()); });

        this.DoActionHelper(orderedPlayers, 0);
        
    }


    public void DoActionHelper(List<FightingEntity> orderedEntities, int curIndex) {
        if (curIndex == orderedEntities.Count) {
            this.OnPlayerTurnStart();
            return;
        }
        
        FightingEntity entity = orderedEntities[curIndex];
        if (entity.GetType() == typeof(Enemy)) {
            entity.SetQueuedAction(new QueuedAction(entity, entity.GetRandomAction(), new List<int>{0}, TargetType.PLAYER  ));

        } else {
        }

        StartCoroutine(dummyAttackAnimation(entity, orderedEntities, curIndex));
        
    }

    public int getRandomEnemyTarget() {
        int enemyTarget = Random.Range(0, GetPlayers().Count);
        return enemyTarget;
    }

    IEnumerator dummyAttackAnimation(FightingEntity a, List<FightingEntity> orderedEntities, int index) {

        string attackerName = a.Name;
        if (a.GetQueuedAction() == null) {
            Debug.LogError("Cannot find queued action: "  + attackerName);
            yield break;
        }
        string attackName = a.GetQueuedAction()._action.name;

        CommentaryText.text = "" + attackerName + " used " + attackName;
        yield return new WaitForSeconds(2f);
        this.DoAction(a);
        this.DoActionHelper(orderedEntities, index + 1);
    }

    public void OnPlayerTurnStart() {
        this.startActions = false;
        
        this.ActionMenu.gameObject.SetActive(true);
        this.CommentaryMenu.gameObject.SetActive(false);

        this.playerActionCounter = 0;
        foreach (var player in GetPlayers()) {
            player.ResetCommand();

            if (player.modifiers.Contains("defend")) {
                player.modifiers.Remove("defend");
            }
        }
    }

    public void DoAction(FightingEntity a) {
        a.GetQueuedAction().ExecuteAction();
    }
}
