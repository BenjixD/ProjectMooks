using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleStageInfo {
    public int minWaves = 1;
    public int maxWaves = 3;
    public List<WaveInfo> waveInfos;
}
