

using UnityEngine;

[CreateAssetMenu(fileName = "Rewards", menuName = "Rewards/PlayerRewardAction", order = 0)]
public class PlayerRewardAction : PlayerReward
{

    public override PlayerRewardType rewardType {
        get { return PlayerRewardType.ACTION; }
    }

    public ActionBase action;

    public override void Initialize(Fighter player) {
        // Note: Randomization/dynamic description should be done here.
        // Be sure to instantiate the scriptable object before you call this
        this.action = Instantiate(action);
    }
}
