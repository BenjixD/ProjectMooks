using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArmouredDefend", menuName = "ActionsAnimations/ArmouredDefend")]
public class ArmouredDefend : ActionAnimation {
    public override void Animate(AnimationController controller) {
        controller.AddToTrack("idle", "defend start", false, 0);
    }
}
