using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

// Class that holds field information / helper functions
public class BattleField : MonoBehaviour
{   
    public Party<PlayerObject> playerParty = new Party<PlayerObject>();
    public Party<EnemyObject> enemyParty = new Party<EnemyObject>(); 


    [Header("Slots")]
    public FighterSlot heroSlot;
    public List<FighterSlot> mookSlots;
    public List<FighterSlot> enemySlots;

    public int currentWaveIndex {get; set;}

    public void Initialize() {

        this.InitializePlayers();
        this.InitializeEnemies();
    }

    public PlayerObject GetHeroPlayer() {
        return playerParty.members[0];
    }

    public void RequestRecruitNewParty() {
        List<int> emptySlots = new List<int>();
        PlayerObject[] players = playerParty.members;
        for (int i = 1; i < players.Length; i++) {
            if (players[i] == null) {
                emptySlots.Add(i);
            }
        }

        GameManager.Instance.party.TryFillAllPartySlots();
        List<PlayerObject> joinedPlayers = new List<PlayerObject>();

        foreach (int emptySlot in emptySlots) {
            PlayerObject player = this.InstantiatePlayer(emptySlot);
            if (player != null) {
                this.mookSlots[emptySlot-1].InitializePosition(player);
                joinedPlayers.Add(player);
            }
        }

        if (joinedPlayers.Count > 0) {
            Messenger.Broadcast<List<PlayerObject>>(Messages.OnPlayersJoinBattle, joinedPlayers);
        }

    }

    public List<FightingEntity> GetAllFightingEntities() {
        List<PlayerObject> players = playerParty.GetActiveMembers();
        List<FightingEntity> entities = new List<FightingEntity>();
        foreach (var player in players) {
            entities.Add(player);
        }

        List<EnemyObject> enemies = enemyParty.GetActiveMembers();

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
            EnemyObject instantiatedEnemy = Instantiate(enemyPrefab) as EnemyObject;

            PlayerStats stats = new PlayerStats(enemyPrefab.stats);
            stats.RandomizeStats();
            stats.ResetStats();
            PlayerCreationData creationData = new PlayerCreationData(enemyPrefab.Name + " (" + GetTargetNameFromIndex(i) + ")", stats, Job.BASIC_ENEMY);
            instantiatedEnemy.Initialize(i, creationData);

            // TODO: Some sort of entrance animation
            
            instantiatedEnemy.GetComponent<MeshRenderer>().sortingOrder = i;
            FighterSlot slot = enemySlots[i];
            slot.InitializePosition(instantiatedEnemy);

            enemyParty.members[i] = instantiatedEnemy;
        }
    }

    public string GetTargetNameFromIndex(int index) {
        char letter = (char) ('A' + index);
        string targetName = letter.ToString().ToUpper();

        return targetName;
    }

    public List<FighterSlot> GetFighterSlots() {
        List<FighterSlot> slots = new List<FighterSlot>();
        slots.Add(heroSlot);
        foreach (var slot in mookSlots) {
            slots.Add(slot);
        }

        foreach (var slot in enemySlots) {
            slots.Add(slot);
        }
        
        return slots;
    }


    private void InitializePlayers() {
        PlayerObject instantiatedHeroPlayer = this.InstantiatePlayer(0);
        this.heroSlot.InitializePosition(instantiatedHeroPlayer);

        for (int i = 1; i < Party<PlayerObject>.maxPlayers; i++) {
            this.InstantiatePlayerIfExists(i);
        }
    }

    private PlayerObject InstantiatePlayerIfExists(int index) {
        PlayerObject instantiatedPlayer = this.InstantiatePlayer(index);
        if (instantiatedPlayer != null) {
            FighterSlot slot;
            if (index == 0) {
                slot = heroSlot;
            } else {
                slot = mookSlots[index-1];
            }

            slot.InitializePosition(instantiatedPlayer);
            Debug.Log("Instantiated player " + instantiatedPlayer.Name + " in slot: " + index);
        } else {
            Debug.Log("Failed to instantiate player in slot: " + index);
        }

        return instantiatedPlayer;
    }

    private void InitializeEnemies() {
        this.GenerateEnemyList(0);
    }

    private PlayerObject InstantiatePlayer(int index) {
        PlayerCreationData data = GameManager.Instance.party.GetPlayerCreationData()[index];
        if (data == null) {
            return null;
        }

        JobActionsList jobActionsList = GameManager.Instance.models.GetPlayerJobActionsList(data.job);
        FightingEntity prefab = jobActionsList.prefab;
        PlayerObject player = Instantiate(prefab).GetComponent<PlayerObject>();
        player.Initialize(index, data);
        playerParty.members[index] = player;

        Debug.Log("Created player: " + data.name);
        Debug.Log("Player actions: " + player.actions);

        return player;
    }

}
