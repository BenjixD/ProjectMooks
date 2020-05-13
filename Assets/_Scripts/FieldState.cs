using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

// Class that holds field information / helper functions
public class FieldState : MonoBehaviour
{   
    public Party<Player> playerParty = new Party<Player>();
    public Party<Enemy> enemyParty = new Party<Enemy>(); 


    [Header("Slots")]
    public Transform heroSlot;
    public List<Transform> mookSlots;
    public List<Transform> enemySlots;

    public int currentWaveIndex {get; set;}

    public void Initialize() {

        this.InitializePlayers();
        this.InitializeEnemies();
    }

    public Player GetHeroPlayer() {
        return playerParty.members[0];
    }

    public void RequestRecruitNewParty() {
        List<int> emptySlots = new List<int>();
        Player[] players = playerParty.members;
        for (int i = 1; i < players.Length; i++) {
            if (players[i] == null) {
                emptySlots.Add(i);
            }
        }

        GameManager.Instance.party.TryFillAllPartySlots();
        List<Player> joinedPlayers = new List<Player>();

        foreach (int emptySlot in emptySlots) {
            Player player = this.InstantiatePlayer(emptySlot);
            if (player != null) {
                joinedPlayers.Add(player);
            }
        }

        if (joinedPlayers.Count > 0) {
            Messenger.Broadcast<List<Player>>(Messages.OnPlayersJoinBattle, joinedPlayers);
        }

    }

    public List<FightingEntity> GetAllFightingEntities() {
        List<Player> players = playerParty.GetActiveMembers();
        List<FightingEntity> entities = new List<FightingEntity>();
        foreach (var player in players) {
            entities.Add(player);
        }

        List<Enemy> enemies = enemyParty.GetActiveMembers();

        foreach (var enemy in enemies) {
            entities.Add(enemy);
        }

        return entities;
    }

    public void GenerateEnemyList(int waveIndex) {
        this.currentWaveIndex = waveIndex;

        StageInfoContainer stageInfo = GameManager.Instance.gameState.GetCurrentStage();
        WaveInfoContainer waveInfo = stageInfo.GetWaveInfo(this.currentWaveIndex);
        waveInfo.InitializeEnemyList(); // Does random generation

        int numberOfEnemiesToGenerate = waveInfo.numEnemies;
        List<JobActionsList> enemyList = waveInfo.GetEnemyList();

        for (int i = 0; i < enemyList.Count; i++) {
            JobActionsList jobList = enemyList[i];

            FightingEntity enemyPrefab = jobList.prefab;
            Enemy instantiatedEnemy = Instantiate(enemyPrefab) as Enemy;

            PlayerStats stats = new PlayerStats(enemyPrefab.stats);
            stats.RandomizeStats();
            stats.ResetStats();
            PlayerCreationData creationData = new PlayerCreationData(enemyPrefab.Name + " (" + i + ")", stats, Job.BASIC_ENEMY);
            instantiatedEnemy.Initialize(i, creationData);

            // TODO: Some sort of entrance animation
            
            instantiatedEnemy.GetComponent<MeshRenderer>().sortingOrder = i;
            instantiatedEnemy.transform.SetParent(enemySlots[i]);
            instantiatedEnemy.transform.localPosition = Vector3.zero;

            enemyParty.members[i] = instantiatedEnemy;
        }
    }

    private void InitializePlayers() {
        Player instantiatedHeroPlayer = this.InstantiatePlayer(0);
        instantiatedHeroPlayer.transform.SetParent(heroSlot, false);
        instantiatedHeroPlayer.transform.localPosition = Vector3.zero;

        for (int i = 1; i < Party<Player>.maxPlayers; i++) {
            this.InstantiatePlayerIfExists(i);
        }
    }

    private Player InstantiatePlayerIfExists(int index) {
        Player instantiatedPlayer = this.InstantiatePlayer(index);
        if (instantiatedPlayer != null) {
            instantiatedPlayer.transform.SetParent(mookSlots[index-1].transform, false);
            instantiatedPlayer.transform.localPosition = Vector3.zero;
            Debug.Log("Instantiated player " + instantiatedPlayer.Name + " in slot: " + index);
        } else {
            Debug.Log("Failed to instantiate player in slot: " + index);
        }

        return instantiatedPlayer;
    }

    private void InitializeEnemies() {
        this.GenerateEnemyList(0);
    }

    private Player InstantiatePlayer(int index) {
        PlayerCreationData data = GameManager.Instance.party.GetPlayerCreationData()[index];
        if (data == null) {
            return null;
        }

        JobActionsList jobActionsList = GameManager.Instance.models.GetPlayerJobActionsList(data.job);
        FightingEntity prefab = jobActionsList.prefab;
        Player player = Instantiate(prefab).GetComponent<Player>();
        player.Initialize(index, data);
        playerParty.members[index] = player;

        Debug.Log("Created player: " + data.name);
        Debug.Log("Player actions: " + player.actions);

        return player;
    }
}
