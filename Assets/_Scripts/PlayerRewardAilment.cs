

using UnityEngine;

[CreateAssetMenu(fileName = "Rewards", menuName = "Rewards/PlayerRewardAiment", order = 0)]
public class PlayerRewardAilment : PlayerReward
{

    public override PlayerRewardType rewardType {
        get { return PlayerRewardType.AILMENT; }
    }

    public StatusAilment ailment;

    public override void Initialize(Fighter player) {
        // Note: Randomization/dynamic description should be done here.
        // Be sure to instantiate the scriptable object before you call this
        this.ailment = Instantiate(ailment);
    }

    public virtual StatusAilment OnAttachBuff(FightingEntity player) {
        return this.ailment;
    }
}
