using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClericCast", menuName = "ActionsAnimations/ClericCast")]
public class ClericCast : ActionAnimation {
    protected override void AnimateUser(FightingEntity user) {
        AnimationController controller = user.GetAnimController();
        int track = controller.TakeFreeTrack();
        controller.AddToTrack(track, "heal start", false, 0);
        controller.AddToTrack(track, "heal end", false, 1.5f);
        controller.EndTrackAnims(track);
    }
}
