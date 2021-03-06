﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HeroTomeFlip", menuName = "ActionsAnimations/HeroTomeFlip")]
public class HeroTomeFlip : ActionAnimation {
    protected override void AnimateUser(FightingEntity user) {
        // Perform casting spell animation for hero
        AnimationController controller = user.GetAnimController();
        int bookTrack = controller.TakeFreeTrack();
        controller.AddToTrack("idle pose", "magic attack start", false, 0);
        controller.AddToTrack(bookTrack, "book open", false, 0.5f);
        controller.AddToTrack(bookTrack, "book flip", true, 1f);
        controller.AddToTrack(bookTrack, "book close", false, 2f);
        controller.EndTrackAnims(bookTrack);
        controller.AddToTrack("idle pose", "magic attack end", false, 4f);
        controller.AddToTrack("idle pose", "idle pose", true, 0f);
    }
}
