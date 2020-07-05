using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewActionAnimation", menuName = "ActionsAnimations/DefaultActionAnimation", order = 0)]
public class ActionAnimation : ScriptableObject {
    [Tooltip("The name of the default animation played for the user of this action. Alternatively, you can override Animate().")]
    public string userAnimName;
    [Tooltip("Time into the action before the effect takes place.")]
    public float timeBeforeEffect;
    [Tooltip("Time to wait after the action effect takes place.")]
    public float timeAfterEffect;

    public virtual void Animate(AnimationController controller) {
        int track = controller.TakeFreeTrack();
        if (track != -1) {
            controller.AddToTrack(track, userAnimName, false, 0);
            controller.EndTrackAnims(track);
        }
    }
}
