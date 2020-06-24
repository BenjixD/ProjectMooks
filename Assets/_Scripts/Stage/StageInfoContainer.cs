
using UnityEngine;
using System.Collections.Generic;

public class StageInfoContainer
{
    
    // Difficulty change for each increase or decrease in difficulty level from Medium
    public static float difficultyMult = 0.3f;
    
    public int numWaves = 0;

    private StageInfo _stageInfo;
    private BattleType _battleType;
    private List<WaveInfoContainer> _waveInfo;
    
    public StageInfoContainer(StageInfo stageInfo, BattleType battleType) {
        _stageInfo = stageInfo;
        _battleType = battleType;
        numWaves = _stageInfo.waveInfos.Count;
        // Randomly generate waves
        InitializeWaveList();
    }

    private void InitializeWaveList() {
        _waveInfo = new List<WaveInfoContainer>();
        for (int i = 0; i < numWaves; i++) {
            _waveInfo.Add(new WaveInfoContainer(_stageInfo.waveInfos[i % numWaves]));
        }
    }

    public float GetDifficultyMult() {
        switch(_battleType) {
            case BattleType.EASY_BATTLE:
                return -difficultyMult;
            case BattleType.HARD_BATTLE:
                return difficultyMult;
            default:
                return 0;
        }
    }

    // TODOL
    // private void ApplyDifficulty(BattleType battleType) {
    //     switch(battleType) {
    //         case BattleType.EASY_BATTLE:
    //             MultiplyStats(-difficultyMult);
    //             break;
    //         case BattleType.HARD_BATTLE:
    //             MultiplyStats(difficultyMult);
    //             break;
    //     }
    // }

    // TODOL
    // private void MultiplyStats(float mult) {
    //     mult += 1;
    //     if (mult <= 0) {
    //         Debug.LogWarning("Bad stat multiplier: " + mult);
    //         return;
    //     }
    //     // Adjust all the stats of every enemy
    //     List<FightingEntity> enemies = new List<FightingEntity>(GameManager.Instance.battleComponents.field.GetActiveEnemyObjects());
    //     Debug.Log("enemy count: " + enemies.Count);
    //     foreach (EnemyObject enemy in enemies) {
    //         PlayerStats stats = enemy.stats;
    //         foreach(Stat stat in System.Enum.GetValues(typeof(Stat))) {
    //             stats.ModifyStat(stat, (int)(stats.GetStat(stat) * mult));
    //         }
    //     }
    // }

    public WaveInfoContainer GetWaveInfo(int waveIndex) {
        return _waveInfo[waveIndex];
    }
}