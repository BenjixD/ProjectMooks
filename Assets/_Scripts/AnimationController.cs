using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class AnimationController : MonoBehaviour {
    private SkeletonAnimation _skeletonAnimation;
    private Spine.AnimationState _animState;
    [SerializeField, SpineAnimation] private string _defaultAnimName = null;
    private HashSet<string> _animations = new HashSet<string>();

    private void Start() {
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
        _animState = _skeletonAnimation.state;
        // Store all the names of this skeleton's anims
        foreach (Spine.Animation anim in _animState.Data.SkeletonData.Animations) {
            _animations.Add(anim.Name);
        }
        SetAnimation(_defaultAnimName, true);
    }

    public void SetAnimation(string animationName, bool loop) {
        if (!_animations.Contains(animationName)) {
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
        SetAnimation(_defaultAnimName, true);
    }
}
