

using UnityEngine;
using System.Collections.Generic;


public enum PlayerRewardType {
    OTHER,
    AILMENT, // Note: Most rewards will most likely be ailment
    ACTION
}
public abstract class PlayerReward : ScriptableObject
{
    public new string name;
    public string description;
    public RewardRarity rarity;
    public List<RewardAffinity> affinityTypes;

    public bool isUnique = true;

    public abstract PlayerRewardType rewardType {get;}


    public virtual void Initialize(Fighter player) {
        // Note: Randomization/dynamic description should be done here.
        // Be sure to instantiate the scriptable object before you call this
    }
}
