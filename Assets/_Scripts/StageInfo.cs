using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class StageInfo : MonoBehaviour
{
    private Enemy[] _enemies = new Enemy[Party.numPlayers];


    [Header("Slots")]
    public Transform heroSlot;
    public List<Transform> mookSlots;
    public List<Transform> enemySlots;

    Player _heroPlayer;

    public void Initialize() {
        Messenger.AddListener<DeathResult>(Messages.OnEntityDeath, this.onEntityDeath);

        this.InitializePlayers();
        this.InitializeEnemies();
    }

    void OnDestroy() {
        Messenger.RemoveListener<DeathResult>(Messages.OnEntityDeath, this.onEntityDeath);
    }

    public Player GetHeroPlayer() {
        return _heroPlayer;
    }
    public Enemy[] GetEnemies() {
        return _enemies;
    }

    public Player[] GetPartyPlayers() {
        return GameManager.Instance.party.GetPlayersInPosition();
    }

    public List<Player> GetActivePlayers() {
        List<Player> activePlayers = new List<Player>();
        Player[] players = GetPartyPlayers();
        for (int i = 0; i < players.Length; i++) {
            if (players[i] != null) {
                activePlayers.Add(players[i]);
            }
        }

        return activePlayers;
    }

    public List<Enemy> GetActiveEnemies() {
        List<Enemy> activeEnemies = new List<Enemy>();
        Enemy[] enemies = GetEnemies();
        for (int i = 0; i < enemies.Length; i++) {
            if (enemies[i] != null) {
                activeEnemies.Add(enemies[i]);
            }
        }

        return activeEnemies;
    }

    public List<FightingEntity> GetAllFightingEntities() {
        List<Player> players = GetActivePlayers();
        List<FightingEntity> entities = new List<FightingEntity>();
        foreach (var player in players) {
            entities.Add(player);
        }

        List<Enemy> enemies = GetActiveEnemies();

        foreach (var enemy in enemies) {
            entities.Add(enemy);
        }

        return entities;
    }

    public int GetRandomEnemyIndex() {
        List<Enemy> enemies = GetActiveEnemies();
        return enemies[Random.Range(0, enemies.Count)].targetId;
    }

    public int GetRandomPlayerIndex() {
        List<Player> players = GetActivePlayers();
        return players[Random.Range(0, players.Count)].targetId;
    }

    public void RequestRecruitNewParty() {
        List<int> emptySlots = new List<int>();
        Player[] players = GetPartyPlayers();
        for (int i = 1; i < players.Length; i++) {
            if (players[i] == null) {
                emptySlots.Add(i);
            }
        }

        GameManager.Instance.party.TryFillAllPartySlots();

        foreach (int emptySlot in emptySlots) {
            this.InstantiatePlayer(emptySlot, true);
        }
    }

    private void onEntityDeath(DeathResult result) {
        FightingEntity deadFighter = result.deadEntity.fighter;
        // TODO: Play death animation
        


        bool isEnemy = deadFighter.isEnemy();
        if (isEnemy) {
            _enemies[deadFighter.targetId] = null;
            Destroy(deadFighter.gameObject);
        } else {

            if (deadFighter.targetId == 0) {
                this.onHeroDeath(result);
                return;
            }

            GameManager.Instance.party.EvictPlayer(deadFighter.Name);
            Destroy(deadFighter.gameObject);
        }

    }

    private void onHeroDeath(DeathResult result) {
        // TODO: end the game
    } 


    private void InitializePlayers() {
        Player instantiatedHeroPlayer = GameManager.Instance.party.InstantiatePlayer(0);
        instantiatedHeroPlayer.transform.SetParent(heroSlot, false);
        instantiatedHeroPlayer.transform.localPosition = Vector3.zero;

        for (int i = 1; i < Party.numPlayers; i++) {
            this.InstantiatePlayer(i, false);
        }

        _heroPlayer = GetPartyPlayers()[0];
    }

    private void InstantiatePlayer(int index, bool broadcastJoin) {
        Player instantiatedPlayer = GameManager.Instance.party.InstantiatePlayer(index);
        if (instantiatedPlayer != null) {
            instantiatedPlayer.transform.SetParent(mookSlots[index-1].transform, false);
            instantiatedPlayer.transform.localPosition = Vector3.zero;
            Debug.Log("Instantiated player " + instantiatedPlayer.Name + " in slot: " + index);

            if (broadcastJoin) {
                Messenger.Broadcast<Player>(Messages.OnPlayerJoinBattle, instantiatedPlayer);
            }
        } else {
            Debug.Log("Failed to instantiate player in slot: " + index);
        }

    }

    private void InitializeEnemies() {
        this.GenerateEnemyList();
    }


    private void GenerateEnemyList() {
        int numberOfEnemiesToGenerate = 3; // TODO: Make this dependent on stage.
        List<Job> enemyJobList = GameManager.Instance.enemyJobActions.Keys.ToList();

        for (int i = 0; i < numberOfEnemiesToGenerate; i++) {
            int enemyTypeIndex = Random.Range(0, enemyJobList.Count);

            JobActionsList jobList = GameManager.Instance.GetEnemyJobActionsList(enemyJobList[enemyTypeIndex]);


            FightingEntity enemyPrefab = jobList.prefab;
            Enemy instantiatedEnemy = Instantiate(enemyPrefab) as Enemy;

            PlayerStats stats = new PlayerStats(enemyPrefab.stats);
            stats.RandomizeStats();
            stats.ResetStats();
            PlayerCreationData creationData = new PlayerCreationData(enemyPrefab.Name + " (" + i + ")", stats, Job.BASIC_ENEMY);
            instantiatedEnemy.Initialize(i, creationData);
            

            instantiatedEnemy.GetComponent<MeshRenderer>().sortingOrder = i;
            instantiatedEnemy.transform.SetParent(enemySlots[i]);
            instantiatedEnemy.transform.localPosition = Vector3.zero;

            _enemies[i] = instantiatedEnemy;
        }
    }


}
