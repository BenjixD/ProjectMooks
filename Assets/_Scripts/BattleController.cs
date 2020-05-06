﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum BattlePhase {
    PLAYER,
    ENEMY

};
public enum HeroInputActionState {
    SELECT_ACTION,
    SELECT_ENEMY_TARGET,
    SELECT_PLAYER_TARGET,
    BATTLE_START
}


public class BattleController : MonoBehaviour
{

    public CommandSelector commandSelector;

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

    
    public Transform heroSlot;
    public List<Transform> mookSlots;
    public List<Transform> enemySlots;

    public Color targetSelectionColor;



    Player _heroPlayer;
    int heroActionIndex = 0;
    public List<RectTransform> battleOptionsUI {get ; set; }

    public bool inputActionsPhase{get; set; }

    public RectTransform playerStatusBarParent;

    public RectTransform enemyStatusBarParent;

    public StatusBarUI statusBarPrefab;

    public Text stateText;

    private List<StatusBarUI> statusBars;
    private List<StatusBarUI> enemyStatusBars;


    [SerializeField]
    List<Enemy> _enemyPrefabs;

    private HeroInputActionState heroInputActionState = HeroInputActionState.SELECT_ACTION;
    private int heroTargetIndex = 0;


    void Start() {
        GameManager.Instance.battleController = this;

        // Instantiate hero
        
        Player instantiatedHeroPlayer = GameManager.Instance.party.InstantiatePlayer(0);
        instantiatedHeroPlayer.transform.SetParent(heroSlot);
        instantiatedHeroPlayer.transform.localPosition = Vector3.zero;


        int numPlayersInParty = GameManager.Instance.party.GetNumPlayersInParty();
        for (int i = 1; i < numPlayersInParty; i++) {
            Player instantiatedPlayer = GameManager.Instance.party.InstantiatePlayer(i);
            instantiatedPlayer.transform.SetParent(mookSlots[i].transform);
            instantiatedPlayer.transform.localPosition = Vector3.zero;
        }


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


    // Update is called once per frame
    void Update()
    {

        if (inputActionsPhase == true) {

            switch (heroInputActionState) {
                case HeroInputActionState.SELECT_ACTION:
                    if (Input.GetKeyDown(KeyCode.Z)) {
                        this.heroInputActionState = HeroInputActionState.SELECT_ENEMY_TARGET;
                        this.UpdateTargetSelectionUI();

                        this.commandSelector.Initialize(0, enemies.Count-1, this.UpdateTargetSelectionUI, true);
                    }
                    break;

                case HeroInputActionState.SELECT_ENEMY_TARGET:
                    if (Input.GetKeyDown(KeyCode.Z)) {
                        heroInputActionState = HeroInputActionState.BATTLE_START;
                        this.UpdateTargetSelectionUI();
                        _heroPlayer.SetQueuedAction(new QueuedAction(_heroPlayer, _heroPlayer.actions[heroActionIndex], new List<int>{heroTargetIndex}  ));
                    }
                    break;

                default:

                    break;
            }

            playerActionCounter += Time.deltaTime;
            bool startTurn = (_heroPlayer.HasSetCommand() && playerActionCounter >= this.maxTimeBeforeAction) || hasEveryoneEnteredActions();
            if (startTurn) {
                playerActionCounter = 0;
                this.ExecutePlayerTurn();
            } else if (!_heroPlayer.HasSetCommand()) {
                    stateText.text = "Waiting on streamer input";
            } else {
                stateText.text = "Waiting on Chat: " + (int)(this.maxTimeBeforeAction - playerActionCounter);
            }
        
        } else {
            stateText.text = "";
        }
    }

    private void GenerateEnemyList() {
        int numberOfEnemiesToGenerate = 4; // TODO: Make this dependent on stage.
        enemies = new List<Enemy>();
        List<Enemy> validEnemies = new List<Enemy>(_enemyPrefabs);  // TODO: make validEnemies dependent on the level - best done in a JSON object


        for (int i = 0; i < numberOfEnemiesToGenerate; i++) {
            int enemyIndex = Random.Range(0, validEnemies.Count);

            Enemy enemyPrefab = validEnemies[enemyIndex];
            Enemy instantiatedEnemy = Instantiate(enemyPrefab) as Enemy;

            PlayerStats stats = new PlayerStats(enemyPrefab.stats);
            stats.RandomizeStats();
            stats.ResetStats();
            PlayerCreationData creationData = new PlayerCreationData("Evil monster " + (i+1), stats, Job.BASIC_ENEMY);
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

    private bool hasEveryoneEnteredActions() {
        foreach (var player in GetPlayers()) {
            if (player.HasSetCommand() == false) {
                return false;
            }
        }

        return true;
    }


    public void UpdateBattleOptionUI() {
        this.heroActionIndex = this.commandSelector.GetChoice();
        selectionCursor.transform.parent = battleOptionsUI[heroActionIndex].transform;
        selectionCursor.anchoredPosition = selectionCursorOffset;
    }

    public void UpdateTargetSelectionUI() {

        this.heroTargetIndex = this.commandSelector.GetChoice();

        //TODO: Update this animation
        switch (heroInputActionState) {
            case HeroInputActionState.SELECT_ENEMY_TARGET:
                foreach (var enemy in enemies) {
                    enemy.GetComponent<SpriteRenderer>().color = Color.white;
                }
                enemies[heroTargetIndex].GetComponent<SpriteRenderer>().color = targetSelectionColor;
            break;

            case HeroInputActionState.SELECT_PLAYER_TARGET:
                // TODO: 
                break;

            default:
                foreach (var enemy in enemies) {
                    enemy.GetComponent<SpriteRenderer>().color = Color.white;
                }
                break;
        }
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
            entity.SetQueuedAction(new QueuedAction(entity, entity.GetRandomAction(), new List<int>{0}  ));

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
        this.commandSelector.Initialize(0, _heroPlayer.actions.Count-1, this.UpdateBattleOptionUI);

        this.heroInputActionState = HeroInputActionState.SELECT_ACTION;
        
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
