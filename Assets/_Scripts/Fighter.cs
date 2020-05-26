
using UnityEngine;

public class Fighter
{
    public PlayerCreationData playerCreationData;

    // Player object (null if not on player scene)
    public FightingEntity fighter;


    public T InstantiateFromJob<T>(JobActionsList jobList, string name, int targetId) where T : FightingEntity  {
        T prefab = (T)jobList.prefab;
        T instantiatedFighter = GameObject.Instantiate<T>(prefab) as T;

        PlayerStats stats = new PlayerStats(prefab.stats);
        stats.RandomizeStats();
        stats.ResetStats();
        PlayerCreationData creationData = new PlayerCreationData(name, stats, jobList.job);
        instantiatedFighter.Initialize(targetId, creationData);
        this.fighter = instantiatedFighter;

        return instantiatedFighter;

    }
}
