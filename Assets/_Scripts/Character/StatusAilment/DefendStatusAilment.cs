using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefendStatusAilment", menuName = "StatusAilment/Defend Status Ailment")]
public class DefendStatusAilment : StatChangeStatusAilment {
    public override void Recover(Fighter p) {
		base.Recover(p);
        if (p.fighter == null) {
            Debug.LogError("ERROR: Fighting entity is null!");
        }

        AnimationController controller = p.fighter.GetAnimController();
        controller.AddToTrack("idle", "defend end", false, 0);
        controller.AddToTrack("idle", "idle", true, 0);
	}
}
