using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewClericSpell", menuName = "Actions/ClericSpell", order = 1)]
public class ClericSpell : ActionBase {
    protected override void Animate(AnimationController controller) {
        int track = controller.TakeFreeTrack();
        controller.AddToTrack(track, "heal start", false, 0);
        controller.AddToTrack(track, "heal end", false, 1.5f);
        controller.EndTrackAnims(track);
    }
}