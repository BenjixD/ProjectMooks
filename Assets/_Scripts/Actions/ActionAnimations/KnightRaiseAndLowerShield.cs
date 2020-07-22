﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KnightRaiseAndLowerShield", menuName = "ActionsAnimations/KnightRaiseAndLowerShield")]
public class KnightRaiseAndLowerShield : ActionAnimation {
    protected override void AnimateUser(FightingEntity user) {
        AnimationController controller = user.GetAnimController();
        controller.AddToTrack("idle", "defend start", false, 0);
        controller.AddToTrack("idle", "defend end", false, 2f);
        controller.AddToTrack("idle", "idle", true, 0);
    }
}
