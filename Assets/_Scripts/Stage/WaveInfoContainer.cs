
using UnityEngine;
using System.Collections.Generic;


public class WaveInfoContainer {
    private int _numEnemies;
    private WaveInfo _waveInfo;

    private List<JobActionsList> enemies;
    
    private void ConstructContainer(WaveInfo waveInfo, int enemies) {
        _waveInfo = waveInfo;
        _numEnemies = enemies;
        InitializeEnemyList();
    }

    public WaveInfoContainer(WaveInfo waveInfo) {
        int enemies = Random.Range(waveInfo.minAmountOfEnemies, waveInfo.maxAmountOfEnemies);
        ConstructContainer(waveInfo, enemies);
    }

    public WaveInfoContainer(WaveInfo waveInfo, int enemyQuantityMod) {
        int enemies = Random.Range(waveInfo.minAmountOfEnemies, waveInfo.maxAmountOfEnemies);
        // Add (or subtract) the given modifier to the number of enemies while keeping it clamped between the min and max
        enemies += enemyQuantityMod;
        enemies = Mathf.Clamp(enemies, waveInfo.minAmountOfEnemies, waveInfo.maxAmountOfEnemies);
        ConstructContainer(waveInfo, enemies);
    }

    public void InitializeEnemyList() {
        enemies = new List<JobActionsList>();

        for (int i = 0; i < _numEnemies; i++) {
            enemies.Add(_waveInfo.possibleEnemies[Random.Range(0, _waveInfo.possibleEnemies.Count)]);
        }
    }

    public WaveInfo GetWaveInfo() {
        return _waveInfo;
    }

    public List<JobActionsList> GetEnemyList() {
        return enemies;
    }
}