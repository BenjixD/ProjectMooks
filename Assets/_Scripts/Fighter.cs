
using UnityEngine;
using System.Collections.Generic;


public class Fighter
{
    public PlayerCreationData playerCreationData;

    // Player object (null if not on player scene)
    public FightingEntity fighter;
    private List<PlayerReward> permanentRewards = new List<PlayerReward>();


    public void SetPlayerCreationData(PlayerCreationData data) {
        this.playerCreationData = data;
    }


    public T InstantiateFromJob<T>(JobActionsList jobList, string name, int targetId) where T : FightingEntity  {
        T prefab = (T)jobList.prefab;
        T instantiatedFighter = GameObject.Instantiate<T>(prefab) as T;

        // Will be null for enemies, but not for players because we want to keep stats
        if (this.playerCreationData == null) {
            PlayerStats stats = (PlayerStats)prefab.stats.Clone();
            stats.RandomizeStats();
            stats.ResetStats();
            this.playerCreationData = new PlayerCreationData(name, stats, jobList.job);
        }

        instantiatedFighter.Initialize(targetId, this.playerCreationData);

        foreach (PlayerReward reward in this.GetPermanentRewards()) {

            switch (reward.rewardType) {
                case PlayerRewardType.AILMENT:
                    PlayerRewardAilment ailmentReward = (PlayerRewardAilment)reward;
                    instantiatedFighter.GetAilmentController().AddStatusAilment(GameObject.Instantiate(ailmentReward.ailment));
                    break;

                default:
                    break;
            }
        }

        this.fighter = instantiatedFighter;

        return instantiatedFighter;

    }

    public List<PlayerReward> GetPermanentRewards() {
        return this.permanentRewards;
    }

    public void AddPermanentReward(PlayerReward reward) {
        this.permanentRewards.Add(reward);
    }
}
