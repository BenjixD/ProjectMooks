

using UnityEngine;

[CreateAssetMenu(fileName = "Rewards", menuName = "Rewards/PlayerReward", order = 0)]
public class PlayerReward : ScriptableObject
{
    public string name;
    public string description;
    public StatusAilment ailment;


    public virtual void Initialize() {
        // Note: Randomization/dynamic description should be done here.
        // Be sure to instantiate the scriptable object before you call this
    }

    public virtual StatusAilment OnAttachBuff(FightingEntity player) {
        return this.ailment;
    }
}
