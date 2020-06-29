using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHeroSpell", menuName = "Actions/HeroSpell", order = 1)]
public class HeroSpell : ActionBase {
    protected override void Animate(AnimationController controller) {
        // TODOL
        // user.Animate(userAnimName, false);
        // foreach(string name in userAnimNamesTest) {
            // user.AddAnimation(name, false);
        // }
        Spine.AnimationState state = controller._animState;
        // Spine.TrackEntry animationEntry = controller._animState.AddAnimation(trackIndex, animationName, loop, delay);
        // Spine.TrackEntry animationEntry = controller._animState.AddAnimation(trackIndex, animationName, loop, delay);

        // if (userAnimName != "") {
        //     int track = controller.AddLayer(userAnimName, false, 0);
        //     if (track != -1) {
        //         Debug.Log("anim track: " + track);
        //         controller.EndTrackAnims(track);
        //     }
        // } else {

            // TODOL: track 3 for book doesn' twork
            int bookTrack = controller.TakeFreeTrack();
            Debug.Log("book track: " + bookTrack);


            controller.AddToTrack("idle pose", "magic attack start", false, 0);
            controller.AddToTrack(bookTrack, "book open", false, 0.5f);
            controller.AddToTrack(bookTrack, "book flip", true, 1f);
            // controller.AddToTrack(2, "book flip", false);
            // controller.AddToTrack(2, "book flip", false);
            // controller.AddToTrack(2, "book flip", false);
            // controller.AddToTrack(2, "book flip", false);
            controller.AddToTrack(bookTrack, "book close", false, 2f);
            // controller.AddToTrack(4, "book idle", true, 0f);
            controller.EndTrackAnims(bookTrack);
            controller.AddToTrack("idle pose", "magic attack end", false, 4f);
            // controller.AddToTrack(3, "book idle", false, 5f);
            controller.AddToTrack("idle pose", "idle pose", true, 0f);


            // user.AddToTrack(7, "magic attack start", true, 0f);
            // user.AddToTrack(8, "magic attack end", true, 0f);
            // user.AddToTrack(9, "book flip", true, 0f);
            // user.AddToTrack(10, "melee attack", true, 0f);
        // }
    }
}
