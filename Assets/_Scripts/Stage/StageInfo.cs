
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WaveInfo {
    public int minAmountOfEnemies = 1;
    public int maxAmountOfEnemies = 1;
    public List<JobActionsList> possibleEnemies;
    // May want to do more in the future
}

[System.Serializable]
public class StageInfo {
    // TODOL
    // public string stageName;
    // public int minAmountWaves = 1;
    // public int maxAmountWaves = 3;
    // TODOL: store difficulty for rewards?
    public StageCategory stageType;
    public List<WaveInfo> waveInfos;
}