﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

// Class that holds field information / helper functions
public class BattleField : MonoBehaviour
{   
    [Header("Slots")]
    public FighterSlot heroSlot;
    public List<FighterSlot> mookSlots;
    public List<FighterSlot> enemySlots;

    public int currentWaveIndex {get; set;}


    public void Initialize() {

        this.InitializePlayers();
        this.InitializeEnemies();
    }

    
    void Update() {
        if (Input.GetKeyDown(KeyCode.K)) {
            List<EnemyObject> enemies = this.GetActiveEnemyObjects();
            foreach(EnemyObject enemy in enemies) {
                enemy.stats.hp.SetValue(0);
                DeathResult deathResult = new DeathResult(enemy, new DamageReceiver(enemy, enemy.stats, enemy.stats), null);
                Messenger.Broadcast<DeathResult>(Messages.OnEntityDeath, deathResult);
            }
        }

        if (Input.GetKeyDown(KeyCode.O)) {
            FightingEntity hero = this.GetHeroPlayer();
            hero.stats.hp.SetValue(0);
            DeathResult deathResult = new DeathResult(hero, new DamageReceiver(hero, hero.stats, hero.stats), null);
            Messenger.Broadcast<DeathResult>(Messages.OnEntityDeath, deathResult);
        }
    }

    // Field helper functions ===
    public PlayerObject GetHeroPlayer() {
        return GameManager.Instance.gameState.playerParty.GetFighterObjects<PlayerObject>()[0];
    }

    public PlayerObject[] GetPlayerObjects() {
        return GameManager.Instance.gameState.playerParty.GetFighterObjects<PlayerObject>();
    }

    public List<PlayerObject> GetActivePlayerObjects() {
        return GameManager.Instance.gameState.playerParty.GetActiveFighterObjects<PlayerObject>();
    }

    public EnemyObject[] GetEnemyObjects() {
        return GameManager.Instance.gameState.enemyParty.GetFighterObjects<EnemyObject>();
    }

    public List<EnemyObject> GetActiveEnemyObjects() {
        return GameManager.Instance.gameState.enemyParty.GetActiveFighterObjects<EnemyObject>();
    }

    public List<FightingEntity> GetAllFightingEntities() {
        List<PlayerObject> players = GetActivePlayerObjects();
        List<FightingEntity> entities = new List<FightingEntity>();
        foreach (var player in players) {
            entities.Add(player);
        }

        List<EnemyObject> enemies = GetActiveEnemyObjects();

        foreach (var enemy in enemies) {
            entities.Add(enemy);
        }

        return entities;
    }

    public int GetRandomPlayerObjectIndex() {
        return GameManager.Instance.gameState.playerParty.GetRandomActiveIndex();
    }

    public int GetRandomEnemyObjectIndex() {
        return GameManager.Instance.gameState.enemyParty.GetRandomActiveIndex();
    }

    // ===


    public void RequestRecruitNewParty() {
        List<int> emptySlots = new List<int>();
        PlayerObject[] players = GetPlayerObjects();
        for (int i = 1; i < players.Length; i++) {
            if (players[i] == null) {
                emptySlots.Add(i);
            }
        }

        GameManager.Instance.gameState.playerParty.TryFillAllPartySlots();
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
            Messenger.Broadcast(Messages.OnUpdateStatusBarsUI);
        }

    }


    public void GenerateEnemyList(int waveIndex) {
        this.currentWaveIndex = waveIndex;

        StageInfoContainer stageInfo = GameManager.Instance.gameState.GetCurrentStage();
        WaveInfoContainer waveInfo = stageInfo.GetWaveInfo(this.currentWaveIndex);

        List<JobActionsList> enemyList = waveInfo.GetEnemyList();
        
//        float mult = stageInfo.GetDifficultyMult();
        BattleType battleType = stageInfo.GetBattleType();

        for (int i = 0; i < enemyList.Count; i++) {
            JobActionsList jobList = enemyList[i];
            string name = jobList.prefab.Name + " (" + GetTargetNameFromIndex(i) + ")";
            int index = i;

            Enemy enemy = new Enemy();
            enemy.Initialize(jobList, name);
            EnemyObject instantiatedEnemy = enemy.InstantiateFromJob<EnemyObject>(jobList, name, index);

            // Apply stat adjustments based on difficulty

            PlayerStats stats = instantiatedEnemy.stats;
            /*
            foreach(Stat stat in System.Enum.GetValues(typeof(Stat))) {
                stats.ModifyStat(stat, (int) (stats.GetStat(stat) * mult));
            }
            */
            int level = waveInfo.GetWaveInfo().averageLevel;

            if (battleType == BattleType.HARD_BATTLE) {
                level += 3;
            } else if (battleType == BattleType.EASY_BATTLE) {
                level -= 1;
            } else if (battleType == BattleType.BOSS) {
               // level += 10; // Should be done from inspector side
            }

            stats.ApplyStatsBasedOnLevel(level);

            // TODO: Some sort of entrance animation

            instantiatedEnemy.SetSortingOrder(i);
            FighterSlot slot = enemySlots[i];
            slot.InitializePosition(instantiatedEnemy);

            GameManager.Instance.gameState.enemyParty.SetFighter(i, enemy);
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

        for (int i = 1; i < PlayerParty.maxPlayers; i++) {
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
        Player player = GameManager.Instance.gameState.playerParty.GetFighters<Player>()[index];
        if (player == null) {
            return null;
        }

        JobActionsList jobActionsList = GameManager.Instance.models.GetPlayerJobActionsList(player.playerCreationData.job);
        PlayerObject playerObject = player.InstantiateFromJob<PlayerObject>(jobActionsList, player.playerCreationData.name, index);

        return playerObject;
    }
}
