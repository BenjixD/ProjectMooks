
using UnityEngine;
using System.Collections.Generic;

public class StageInfoContainer
{
    public int numWaves = 0;
    
    private StageInfo stageInfo;
    private List<WaveInfoContainer> waveInfo;
    
    public StageInfoContainer(StageInfo stageInfo) {
        this.stageInfo = stageInfo;
        this.numWaves = Random.Range(this.stageInfo.minAmountWaves, this.stageInfo.maxAmountWaves);
        this.InitializeWaveList();
    }

    public void InitializeWaveList() {
        this.waveInfo = new List<WaveInfoContainer>();
        for (int i = 0; i < this.numWaves; i++) {
            this.waveInfo.Add(new WaveInfoContainer(this.stageInfo.waveInfos[i % this.numWaves]));
        }
    }

    public WaveInfoContainer GetWaveInfo(int waveIndex) {
        return this.waveInfo[waveIndex];
    }
}