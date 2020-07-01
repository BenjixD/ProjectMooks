using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefendStatusAilment", menuName = "StatusAilment/Defend Status Ailment")]
public class DefendStatusAilment : StatChangeStatusAilment {
    public override void Recover(FightingEntity p) {
		base.Recover(p);
        AnimationController controller = p.GetAnimController();
        controller.AddToTrack("idle", "defend end", false, 0);
        controller.AddToTrack("idle", "idle", true, 0);
	}
}
