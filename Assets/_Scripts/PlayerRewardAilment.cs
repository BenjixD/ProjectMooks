

using UnityEngine;

[CreateAssetMenu(fileName = "Rewards", menuName = "Rewards/PlayerRewardAiment", order = 0)]
public class PlayerRewardAilment : PlayerReward
{

    public override PlayerRewardType rewardType {
        get { return PlayerRewardType.AILMENT; }
    }

    public StatusAilment ailment;

    public virtual StatusAilment OnAttachBuff(FightingEntity player) {
        return this.ailment;
    }
}
