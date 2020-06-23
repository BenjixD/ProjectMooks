
using UnityEngine;
using System.Collections.Generic;

public class StageInfoContainer
{
    public int numWaves = 0;
    
    private StageInfo stageInfo;
    private List<WaveInfoContainer> waveInfo;
    
    public StageInfoContainer(StageInfo stageInfo) {
        this.stageInfo = stageInfo;
        numWaves = stageInfo.waveInfos.Count;
        // Randomly generate waves
        this.InitializeWaveList();
    }

    public void InitializeWaveList() {
        this.waveInfo = new List<WaveInfoContainer>();
        Debug.Log("initializing waves");
        for (int i = 0; i < numWaves; i++) {
            this.waveInfo.Add(new WaveInfoContainer(this.stageInfo.waveInfos[i % numWaves]));
        }
    }

    public WaveInfoContainer GetWaveInfo(int waveIndex) {
        return this.waveInfo[waveIndex];
    }
}