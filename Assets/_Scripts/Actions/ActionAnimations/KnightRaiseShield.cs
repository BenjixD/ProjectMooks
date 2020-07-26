using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KnightRaiseShield", menuName = "ActionsAnimations/KnightRaiseShield")]
public class KnightRaiseShield : ActionAnimation {
    protected override void AnimateUser(FightingEntity user) {
        AnimationController controller = user.GetAnimController();
        controller.AddToTrack("idle", "defend start", false, 0);
    }
}
