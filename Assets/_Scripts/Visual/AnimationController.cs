﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class AnimationController : MonoBehaviour {
    private SkeletonAnimation _skeletonAnimation;
    private Spine.AnimationState _animState;
    [SerializeField, SpineAnimation] private string[] _defaultAnims = null;
    private Dictionary<string, int> _defaultAnimTracks = new Dictionary<string, int>();
    private HashSet<string> _animations = new HashSet<string>();
    private int _freeTrackIndex;    // Index of the earliest free track
    private MeshRenderer _meshRenderer;
    private MaterialPropertyBlock _block;
    private float _dieFadeTime = 0.3f;

    private void Start() {
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
        _animState = _skeletonAnimation.state;
        // Store all the names of this skeleton's anims
        foreach (Spine.Animation anim in _animState.Data.SkeletonData.Animations) {
            _animations.Add(anim.Name);
        }
        
        _freeTrackIndex = 0;
        // Set and save all default animations
        for (int i = 0; i < _defaultAnims.Length; i++) {
            AddToTrack(i, _defaultAnims[i], true, 0);
            _defaultAnimTracks.Add(_defaultAnims[i], i);
            _freeTrackIndex++;
        }

        _meshRenderer = GetComponent<MeshRenderer>();
        _block = new MaterialPropertyBlock();
        _meshRenderer.GetPropertyBlock(_block);
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
    }

    public void AddToTrack(int trackIndex, string animationName, bool loop, float delay) {
        if (!AnimationExists(animationName)) {
            return;
        }
        Spine.TrackEntry animationEntry = _animState.AddAnimation(trackIndex, animationName, loop, delay);
    }

    // Add animation to the end of the track that is identified by the specified default anim name
    public void AddToTrack(string track, string animationName, bool loop, float delay) {
        if (!AnimationExists(animationName)) {
            return;
        }
        if (!_defaultAnimTracks.ContainsKey(track)) {
            Debug.Log("No default anim named " + track + " found");
            return;
        }
        Spine.TrackEntry animationEntry = _animState.AddAnimation(_defaultAnimTracks[track], animationName, loop, delay);
    }

    public int TakeFreeTrack() {
        int freeTrack = _freeTrackIndex;
        FindNewFreeTrack();
        return freeTrack;
    }

    // Finds the next empty track and sets it
    private void FindNewFreeTrack() {
        int prospectiveTrack = _freeTrackIndex++;
        while (_animState.GetCurrent(prospectiveTrack) != null) {
            prospectiveTrack++;
        }
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

    public IEnumerator Flash() {
        int fillPhase = Shader.PropertyToID("_FillPhase");
        _block.SetFloat(fillPhase, 1f);
        _meshRenderer.SetPropertyBlock(_block);
        yield return GameManager.Instance.time.GetController().WaitForSeconds(0.1f);
        if (this != null) {
            _block.SetFloat(fillPhase, 0f);
            _meshRenderer.SetPropertyBlock(_block);
        }
    }
    
    public IEnumerator Fade() {
        int fillColour = Shader.PropertyToID("_FillColor");
        _block.SetColor(fillColour, Color.black);
        int fillPhase = Shader.PropertyToID("_FillPhase");
        float elapsedTime = 0;
        float fillAmt = 0;
        while (elapsedTime / _dieFadeTime <= 1) {
            elapsedTime += GameManager.Instance.time.deltaTime;
            fillAmt = Mathf.Lerp(0, 1, elapsedTime / _dieFadeTime);
            _block.SetFloat(fillPhase, fillAmt);
            _meshRenderer.SetPropertyBlock(_block);
            yield return null;
        }
    }

    public void UnFade() {
        int fillColour = Shader.PropertyToID("_FillColor");
        _block.SetColor(fillColour, Color.white);
        int fillPhase = Shader.PropertyToID("_FillPhase");
        _block.SetFloat(fillPhase, 0.03f);
        _meshRenderer.SetPropertyBlock(_block);
    }
}
