
using UnityEngine;
using System.Collections.Generic;

public class StageInfoContainer
{
    // Difficulty change for each increase or decrease in difficulty level from Medium
    public static float difficultyStatMult = 0.3f;
    // Change in number of enemies in a wave for each increase or decrease in difficulty level from Medium
    public static int difficultyEnemyMod = 1;
    
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
            int modifier = GetEnemyModifier();
            WaveInfo waveInfo = _stageInfo.waveInfos[i % numWaves];
            _waveInfo.Add(new WaveInfoContainer(waveInfo, modifier));
        }
    }

    public BattleType GetBattleType() {
        return this._battleType;
    }

    public float GetDifficultyMult() {
        switch(_battleType) {
            case BattleType.EASY_BATTLE:
                return -difficultyStatMult;
            case BattleType.HARD_BATTLE:
                return difficultyStatMult;
            default:
                return 0;
        }
    }

    public int GetEnemyModifier() {
        switch(_battleType) {
            case BattleType.EASY_BATTLE:
                return -difficultyEnemyMod;
            case BattleType.HARD_BATTLE:
                return difficultyEnemyMod;
            default:
                return 0;
        }
    }

    public WaveInfoContainer GetWaveInfo(int waveIndex) {
        return _waveInfo[waveIndex];
    }

    public Dictionary<string, int> GetEnemyTally() {
        Dictionary<string, int> enemies = new Dictionary<string, int>();
        foreach (WaveInfoContainer waveInfoContainer in _waveInfo) {
            foreach (JobActionsList enemy in waveInfoContainer.GetEnemyList()) {
                if (!enemies.ContainsKey(enemy.prefab.Name)) {
                    enemies.Add(enemy.prefab.Name, 1);
                } else {
                    enemies[enemy.prefab.Name]++;
                }
            }
        }
        return enemies;
    }
}