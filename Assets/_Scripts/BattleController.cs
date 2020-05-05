using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum BattlePhase {
    PLAYER,
    ENEMY

};

public class BattleController : MonoBehaviour
{

    public List<Enemy> enemies{get; set;}

    public CommandOptionText commandTextPrefab;
    public RectTransform selectionCursor;
    public Vector2 selectionCursorOffset = new Vector2(0, 0);

    public float maxTimeBeforeAction = 15;
    public float playerActionCounter {get; set;}

    public RectTransform ActionMenu;

    public RectTransform ActionMenuTextParent;

    public RectTransform CommentaryMenu;
    public Text CommentaryText;

    public List<Transform> enemySlots;


    Player _heroPlayer;
    int heroActionIndex = 0;
    public List<RectTransform> battleOptionsUI {get ; set; }

    public bool inputActionsPhase{get; set; }

    public RectTransform playerStatusBarParent;

    public RectTransform enemyStatusBarParent;

    public StatusBarUI statusBarPrefab;

    private List<StatusBarUI> statusBars;
    private List<StatusBarUI> enemyStatusBars;


    [SerializeField]
    List<Enemy> _enemyPrefabs;

    void Start() {
        GameManager.Instance.battleController = this;
        _heroPlayer = GetPlayers()[0];

        // TODO: Waves
        this.GenerateEnemyList();
        battleOptionsUI = new List<RectTransform>();

        foreach (ActionBase action in _heroPlayer.actions) {
            CommandOptionText  commandText = Instantiate(commandTextPrefab) as CommandOptionText;
            commandText.text.text = action.name;
            commandText.transform.parent = ActionMenuTextParent.transform;
            battleOptionsUI.Add(commandText.GetComponent<RectTransform>());
        }

        int count = GetPlayers().Count;

        statusBars = new List<StatusBarUI>();

        for (int i = 0; i < count; i++) { 
            StatusBarUI statusBarForPlayer = Instantiate(statusBarPrefab);
            statusBarForPlayer.transform.parent = playerStatusBarParent;
            statusBars.Add(statusBarForPlayer);
        }

        enemyStatusBars = new List<StatusBarUI>();

        for (int i = 0; i < enemies.Count; i++) {
            StatusBarUI statusBarForPlayer = Instantiate(statusBarPrefab);
            statusBarForPlayer.transform.parent = enemyStatusBarParent;
            enemyStatusBars.Add(statusBarForPlayer);
        }

        this.OnPlayerTurnStart();
        this.UpdateBattleOptionUI();
        this.UpdateStatusBarUI();
    }


    private void GenerateEnemyList() {
        int numberOfEnemiesToGenerate = 1; // TODO: Make this dependent on stage.
        enemies = new List<Enemy>();
        List<Enemy> validEnemies = new List<Enemy>(_enemyPrefabs);  // TODO: make validEnemies dependent on the level - best done in a JSON object


        for (int i = 0; i < numberOfEnemiesToGenerate; i++) {
            int enemyIndex = Random.Range(0, validEnemies.Count);

            Enemy enemyPrefab = validEnemies[enemyIndex];
            Enemy instantiatedEnemy = Instantiate(enemyPrefab) as Enemy;

            PlayerStats stats = new PlayerStats(enemyPrefab.stats);
            stats.RandomizeStats();
            stats.ResetStats();
            PlayerCreationData creationData = new PlayerCreationData("Evil monster", stats, Job.BASIC_ENEMY);
            instantiatedEnemy.Initialize(creationData);
            

            instantiatedEnemy.GetComponent<SpriteRenderer>().sortingOrder = i;
            instantiatedEnemy.transform.SetParent(enemySlots[i]);
            instantiatedEnemy.transform.localPosition = Vector3.zero;

            enemies.Add(instantiatedEnemy);
        }

        if (enemies.Count == 0) {
            Debug.LogError("ERROR: No enemies found!");
        }
    }

    private List<Player> GetPlayers() {
        return GameManager.Instance.party.GetPlayersInPosition();
    }

    // Update is called once per frame
    void Update()
    {

        if (inputActionsPhase == true) {
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
        this.inputActionsPhase = false;
        this.ActionMenu.gameObject.SetActive(false);
        this.CommentaryMenu.gameObject.SetActive(true);

        // Sort by player speed
        List<FightingEntity> orderedPlayers = new List<FightingEntity>(getAllFightingEntities());
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
        this.inputActionsPhase = true;
        
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
        this.UpdateStatusBarUI();
    }

    public void UpdateStatusBarUI() {
        List<Player> players = GetPlayers();
        for (int i = 0; i < players.Count; i++) {
            statusBars[i].SetName(players[i].Name);
            statusBars[i].SetHP(players[i].stats.GetHp(), players[i].stats.maxHp);
        }

        for (int i = 0; i < enemies.Count; i++) {
            enemyStatusBars[i].SetName(enemies[i].Name);
            enemyStatusBars[i].SetHP(enemies[i].stats.GetHp(), enemies[i].stats.maxHp);
        }
    }

    public List<FightingEntity> getAllFightingEntities() {
        List<Player> players = GameManager.Instance.party.GetPlayersInPosition();
        List<FightingEntity> entities = new List<FightingEntity>();
        foreach (var player in players) {
            entities.Add(player);
        }

        foreach (var enemy in enemies) {
            entities.Add(enemy);
        }

        return entities;
    }

}
