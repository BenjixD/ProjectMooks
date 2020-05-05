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
    public List<RectTransform> battleOptionsUI;
    public RectTransform selectionCursor;
    public Vector2 selectionCursorOffset = new Vector2(0, 0);

    public float maxTimeBeforeAction = 15;
    public float playerActionCounter {get; set;}

    public RectTransform ActionMenu;
    public RectTransform CommentaryMenu;
    public Text CommentaryText;


    BattleOption heroBattleOption = BattleOption.ATTACK;
    List<Player> _players;
    Player _heroPlayer;


    bool startActions = false;


    void Start() {
        _players = GameManager.Instance.party.GetPlayersInPosition();
        _heroPlayer = _players[0];

        // TODO: Waves
        GameManager.Instance.GenerateEnemyList();
        this.OnPlayerTurnStart();
    }

    // Update is called once per frame
    void Update()
    {

        if (startActions == false) {
            if (_heroPlayer.HasSetCommand() == false) {
                if (Input.GetKeyDown(KeyCode.DownArrow)) {
                    BattleOption nextBattleOption = (BattleOption)((int) heroBattleOption + 1);
                    if (nextBattleOption == BattleOption.LENGTH) {
                        nextBattleOption = BattleOption.ATTACK;
                    }

                    heroBattleOption = nextBattleOption;

                    this.UpdateBattleOptionUI();
                }

                if (Input.GetKeyDown(KeyCode.UpArrow)) {
                    BattleOption nextBattleOption = heroBattleOption;
                    if ((int) heroBattleOption == 0) {
                        nextBattleOption = (BattleOption) ((int)BattleOption.LENGTH - 1);
                    } else {
                        nextBattleOption = (BattleOption) ((int)nextBattleOption - 1);
                    }

                    heroBattleOption = nextBattleOption;
                    this.UpdateBattleOptionUI();
                }


                if (Input.GetKeyDown(KeyCode.Z)) {

                    switch (heroBattleOption) {
                        case BattleOption.ATTACK:
                        // TODO: Update this to allow for hero target selection.
                        _heroPlayer.SetQueuedAction(new QueuedAction(_heroPlayer, new AttackTest(), new List<int>{0}, TargetType.ENEMY  ));
                        break;

                        case BattleOption.DEFEND:
                        _heroPlayer.SetQueuedAction(new QueuedAction(_heroPlayer, new Defend(), new List<int>{}, TargetType.PLAYER  ));

                        break;

                        case BattleOption.MAGIC:
                        _heroPlayer.SetQueuedAction(new QueuedAction(_heroPlayer, new FireOneTest(), new List<int>{0}, TargetType.ENEMY  ));

                        break;

                    }
                   

                }
            } else {
                
                if (Input.GetKeyDown(KeyCode.Z)) {

                }
            }



            playerActionCounter += Time.deltaTime;

            if (playerActionCounter >= this.maxTimeBeforeAction || hasEveryoneEnteredActions()) {
                playerActionCounter = 0;
                this.ExecutePlayerTurn();
            }
        }
    }

    private bool hasEveryoneEnteredActions() {
        foreach (var player in _players) {
            if (player.HasSetCommand() == false) {
                return false;
            }
        }

        return true;
    }


    public void UpdateBattleOptionUI() {
        selectionCursor.transform.parent = battleOptionsUI[(int)(heroBattleOption)].transform;
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
            entity.SetQueuedAction(new QueuedAction(entity, new AttackTest(), new List<int>{0}, TargetType.PLAYER  ));

        } else {
        }

        StartCoroutine(dummyAttackAnimation(entity, orderedEntities, curIndex));
        
    }

    public int getRandomEnemyTarget() {
        int enemyTarget = Random.Range(0, _players.Count);
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
        foreach (var player in _players) {
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
