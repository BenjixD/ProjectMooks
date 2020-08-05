using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KnightShieldBash", menuName = "ActionsAnimations/KnightShieldBash")]
public class KnightShieldBash : ActionAnimation {
    protected override void AnimateUser(FightingEntity user) {
        AnimationController controller = user.GetAnimController();
        controller.AddToTrack("idle", "defend start", false, 0);
        controller.AddToTrack("idle", "defend end", false, 0.5f);
        controller.AddToTrack("idle", "idle", true, 0);
    }
}