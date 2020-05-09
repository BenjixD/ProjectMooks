
using UnityEngine;
using System.Collections.Generic;


public class WaveInfoContainer
{
    public int numEnemies = 0;
    private WaveInfo waveInfo;

    private List<JobActionsList> enemies;
    
    public WaveInfoContainer(WaveInfo waveInfo) {
        this.waveInfo = waveInfo;
        this.numEnemies = Random.Range(this.waveInfo.minAmountOfEnemies, this.waveInfo.maxAmountOfEnemies);
    }

    public void InitializeEnemyList() {
        enemies = new List<JobActionsList>();

        for (int i = 0; i < this.numEnemies; i++) {
            enemies.Add(waveInfo.possibleEnemies[Random.Range(0, waveInfo.possibleEnemies.Count)]);
        }
    }

    public WaveInfo GetWaveInfo() {
        return this.waveInfo;
    }

    public List<JobActionsList> GetEnemyList() {
        return enemies;
    }
}