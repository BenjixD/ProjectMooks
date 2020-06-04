
using UnityEngine;
using System.Collections.Generic;


public class Fighter
{
    public PlayerCreationData playerCreationData;

    // Player object (null if not on player scene)
    public FightingEntity fighter;
    private List<PlayerReward> permanentBuffs = new List<PlayerReward>();


    public void SetPlayerCreationData(PlayerCreationData data) {
        this.playerCreationData = data;
    }


    public T InstantiateFromJob<T>(JobActionsList jobList, string name, int targetId) where T : FightingEntity  {
        T prefab = (T)jobList.prefab;
        T instantiatedFighter = GameObject.Instantiate<T>(prefab) as T;

        // Will be null for enemies, but not for players because we want to keep stats
        if (this.playerCreationData == null) {
            PlayerStats stats = new PlayerStats(prefab.stats);
            stats.RandomizeStats();
            stats.ResetStats();
            this.playerCreationData = new PlayerCreationData(name, stats, jobList.job);
        }

        instantiatedFighter.Initialize(targetId, this.playerCreationData);
        this.fighter = instantiatedFighter;

        return instantiatedFighter;

    }

    public List<PlayerReward> getPermanentBuffs() {
        return this.permanentBuffs;
    }

    public void setPermanentBuff(PlayerReward reward) {
        this.permanentBuffs.Add(reward);
    }
}
