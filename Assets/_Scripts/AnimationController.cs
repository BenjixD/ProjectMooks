using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class AnimationController : MonoBehaviour {
    private SkeletonAnimation _skeletonAnimation;
    private Spine.AnimationState _animState;
    [SerializeField] private string _defaultAnimName;

    private void Start() {
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
        _animState = _skeletonAnimation.state;
        AddAnimation(_defaultAnimName, true);
    }

    public void AddAnimation(string animationName, bool loop) {
        // TODO: inefficient to call this repeatedly; should cache results
        if (_animState.Data.SkeletonData.FindAnimation(animationName) == null) {
            Debug.Log("No animation named " + animationName + " for " + GetComponent<FightingEntity>().Name);
            return;
        }
        Spine.TrackEntry animationEntry = _animState.SetAnimation(0, animationName, loop);
        // Return to default animation after completion of this one
        if (animationName != _defaultAnimName) {
            animationEntry.Complete += AnimationEntry_Complete;
        }
    }

    private void AnimationEntry_Complete(Spine.TrackEntry trackEntry) {
        AddAnimation(_defaultAnimName, true);
    }
}
