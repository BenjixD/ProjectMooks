using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class AnimationController : MonoBehaviour {
    private SkeletonAnimation _skeletonAnimation;
    // TODOL: set back to private
    public Spine.AnimationState _animState;
    // TODOL: remove
    [SerializeField, SpineAnimation] private string[] _defaultAnims = null;
    private Dictionary<string, int> _defaultAnimTracks = new Dictionary<string, int>();
    private HashSet<string> _animations = new HashSet<string>();
    private int _freeTrackIndex;    // Index of the earliest free track

    private void Start() {
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
        _animState = _skeletonAnimation.state;
        // Store all the names of this skeleton's anims
        foreach (Spine.Animation anim in _animState.Data.SkeletonData.Animations) {
            _animations.Add(anim.Name);
        }
        // TODOL
        // SetAnimation(_defaultAnimName, true);
        
        _freeTrackIndex = 0;
        // Set and save all default animations
        for (int i = 0; i < _defaultAnims.Length; i++) {
            AddToTrack(i, _defaultAnims[i], true, 0);
            _defaultAnimTracks.Add(_defaultAnims[i], i);
            _freeTrackIndex++;
        }

        // TODOL
        // AddToTrack(0, "cape idle", true, 0);
        // AddToTrack(1, "hair idle", true, 0);
        // AddToTrack(2, "idle pose", true, 0);
        // AddToTrack(3, "book idle", true, 0);
    }

    private bool AnimationExists(string animationName) {
        if (!_animations.Contains(animationName)) {
            Debug.Log("No animation named " + animationName + " for " + GetComponent<FightingEntity>().Name);
            return false;
        }
        return true;
    }

    public void SetAnimation(string animationName, bool loop) {
        if (!AnimationExists(animationName)) {
            return;
        }
        Spine.TrackEntry animationEntry = _animState.SetAnimation(0, animationName, loop);
        // Return to default animation after completion of this one
        // TODOL
        // if (animationName != _defaultAnimName) {
        //     animationEntry.Complete += AnimationEntry_Complete;
        // }
    }

    // TODOL
    public void AddAnimation(string animationName, bool loop) {
        if (!AnimationExists(animationName)) {
            return;
        }
        Spine.TrackEntry animationEntry = _animState.AddAnimation(1, animationName, loop, 0);
        // Return to default animation after completion of this one
        // if (animationName != _defaultAnimName) {
            // animationEntry.Complete += AnimationEntry_Complete;
        // }
    }

    public void AddToTrack(int trackIndex, string animationName, bool loop, float delay) {
        if (!AnimationExists(animationName)) {
            return;
        }
        Spine.TrackEntry animationEntry = _animState.AddAnimation(trackIndex, animationName, loop, delay);
        // Return to default animation after completion of this one
        // if (animationName != _defaultAnimName) {
            // animationEntry.Complete += AnimationEntry_Complete;
        // }
    }

    // Add animation to the end of the track that is identified by the specified default anim name
    public void AddToTrack(string track, string animationName, bool loop, float delay) {
        if (!AnimationExists(animationName)) {
            return;
        }
        if (_defaultAnimTracks.ContainsKey(track)) {
            Spine.TrackEntry animationEntry = _animState.AddAnimation(_defaultAnimTracks[track], animationName, loop, delay);
        }
        // Spine.TrackEntry animationEntry = _animState.AddAnimation(trackIndex, animationName, loop, delay);
        // Return to default animation after completion of this one
        // if (animationName != _defaultAnimName) {
            // animationEntry.Complete += AnimationEntry_Complete;
        // }
    }

    // Add given animation to a new layer and return the new track index
    // public int AddLayer(string animationName, bool loop, float delay) {
    //     if (!AnimationExists(animationName)) {
    //         return -1;
    //     }
    //     int retTrack = _freeTrackIndex;
    //     SetFreeTrack();
    //     AddToTrack(retTrack, animationName, loop, delay);
    //     return retTrack;
    // }

    public int TakeFreeTrack() {
        int freeTrack = _freeTrackIndex;
        FindNewFreeTrack();
        return freeTrack;
    }

    // Finds the next empty track and sets it
    private void FindNewFreeTrack() {
        int prospectiveTrack = _freeTrackIndex++;
        while (_animState.GetCurrent(prospectiveTrack) != null) {
            Debug.Log("checking track: " + prospectiveTrack);
            prospectiveTrack++;
        }
        Debug.Log("new free track is: " + prospectiveTrack);
        _freeTrackIndex = prospectiveTrack;
    }

    public void EndTrackAnims(int trackIndex) {
        _animState.AddEmptyAnimation(trackIndex, 0.5f, 0);
        if (trackIndex < _freeTrackIndex) {
            _freeTrackIndex = trackIndex;
        }
        if (trackIndex < _freeTrackIndex) {
            _freeTrackIndex = trackIndex;
        }
    }

    // TODOL
    // private void AnimationEntry_Complete(Spine.TrackEntry trackEntry) {
    //     SetAnimation(_defaultAnimName, true);
    // }
}
