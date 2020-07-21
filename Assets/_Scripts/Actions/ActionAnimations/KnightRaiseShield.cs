using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KnightRaiseShield", menuName = "ActionsAnimations/KnightRaiseShield")]
public class KnightRaiseShield : ActionAnimation {
    public override void Animate(AnimationController controller) {
        controller.AddToTrack("idle", "defend start", false, 0);
    }
}
