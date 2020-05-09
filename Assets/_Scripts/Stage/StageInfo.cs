
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WaveInfo {
    public int minAmountOfEnemies;
    public int maxAmountOfEnemies;
    public List<JobActionsList> possibleEnemies;
    // May want to do more in the future
}

[CreateAssetMenu(fileName = "StageInfo", menuName = "ProjectMooks/StageInfo", order = 0)]
public class StageInfo : ScriptableObject {
    public string stageName;
    public int minAmountWaves = 1;

    public int maxAmountWaves = 3;

    public List<WaveInfo> waveInfos;
}
    