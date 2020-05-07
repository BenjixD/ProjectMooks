using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StageInfo : MonoBehaviour
{
    private List<Enemy> _enemies{get; set;}


    [Header("Slots")]
    public Transform heroSlot;
    public List<Transform> mookSlots;
    public List<Transform> enemySlots;

    // TODO: Maybe this should be an enemy job list instead
    [Header("Prefabs")]
    [SerializeField]
    List<Enemy> _enemyPrefabs;

    Player _heroPlayer;

    public void Initialize() {
        this.InitializePlayers();
        this.InitializeEnemies();
    }

    public Player GetHeroPlayer() {
        return _heroPlayer;
    }
    public List<Enemy> GetEnemies() {
        return _enemies;
    }

    public List<Player> GetPlayers() {
        return GameManager.Instance.party.GetPlayersInPosition();
    }

    public List<FightingEntity> GetAllFightingEntities() {
        List<Player> players = GetPlayers();
        List<FightingEntity> entities = new List<FightingEntity>();
        foreach (var player in players) {
            entities.Add(player);
        }

        List<Enemy> _enemies = GetEnemies();

        foreach (var enemy in _enemies) {
            entities.Add(enemy);
        }

        return entities;
    }

    public int GetRandomEnemyIndex() {
        List<int> validIndices = new List<int>();
        List<Enemy> enemies = GetEnemies();
        for (int i = 0; i < enemies.Count; i++) {
            if (enemies[i] != null) {
                validIndices.Add(i);
            }
        }

        return validIndices[Random.Range(0, validIndices.Count)];
    }

    public int GetRandomPlayerIndex() {
        int enemyTarget = Random.Range(0, GetPlayers().Count);
        return enemyTarget;
    }


    private void InitializePlayers() {
        Player instantiatedHeroPlayer = GameManager.Instance.party.InstantiatePlayer(0);
        instantiatedHeroPlayer.transform.SetParent(heroSlot);
        instantiatedHeroPlayer.transform.localPosition = Vector3.zero;

        int numPlayersInParty = GameManager.Instance.party.GetNumPlayersInParty();
        for (int i = 1; i < numPlayersInParty; i++) {
            Player instantiatedPlayer = GameManager.Instance.party.InstantiatePlayer(i);
            instantiatedPlayer.transform.SetParent(mookSlots[i-1].transform);
            instantiatedPlayer.transform.localPosition = Vector3.zero;
        }

        _heroPlayer = GetPlayers()[0];
    }

    private void InitializeEnemies() {
        _enemies = new List<Enemy>();
        this.GenerateEnemyList();
    }


    private void GenerateEnemyList() {
        int numberOfEnemiesToGenerate = 4; // TODO: Make this dependent on stage.
        List<Enemy> validEnemies = new List<Enemy>(_enemyPrefabs);  // TODO: make validEnemies dependent on the level - best done in a JSON object

        for (int i = 0; i < numberOfEnemiesToGenerate; i++) {
            int enemyIndex = Random.Range(0, validEnemies.Count);

            Enemy enemyPrefab = validEnemies[enemyIndex];
            Enemy instantiatedEnemy = Instantiate(enemyPrefab) as Enemy;

            PlayerStats stats = new PlayerStats(enemyPrefab.stats);
            stats.RandomizeStats();
            stats.ResetStats();
            PlayerCreationData creationData = new PlayerCreationData("Evil monster " + i, stats, Job.BASIC_ENEMY);
            instantiatedEnemy.Initialize(creationData);
            

            instantiatedEnemy.GetComponent<MeshRenderer>().sortingOrder = i;
            instantiatedEnemy.transform.SetParent(enemySlots[i]);
            instantiatedEnemy.transform.localPosition = Vector3.zero;

            _enemies.Add(instantiatedEnemy);
        }

        if (_enemies.Count == 0) {
            Debug.LogError("ERROR: No _enemies found!");
        }
    }


}
