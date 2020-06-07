

using UnityEngine;



public enum PlayerRewardType {
    OTHER,
    AILMENT // Note: Most rewards will most likely be ailment
}
public abstract class PlayerReward : ScriptableObject
{
    public string name;
    public string description;

    public abstract PlayerRewardType rewardType {get;}


    public virtual void Initialize() {
        // Note: Randomization/dynamic description should be done here.
        // Be sure to instantiate the scriptable object before you call this
    }
}
