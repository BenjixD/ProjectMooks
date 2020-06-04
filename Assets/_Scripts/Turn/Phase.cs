using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public enum TurnPhase{
    TURN_START,
    PARTY_SETUP,
    MOVE_SELECTION,
    BATTLE,
    TURN_END,
    NONE
};

[System.Serializable]
public abstract class Phase {
    public TurnPhase _phase;
    protected string _callback;

    public Phase(string callback) {
        _phase = TurnPhase.NONE;
        _callback = callback;
    }

    public Phase(TurnPhase phase, string callback) {
        _phase = phase;
        _callback = callback;
    }

    public IEnumerator RunPhase() {
        OnPhaseStart();
        yield return GameManager.Instance.StartCoroutine(Run());
        OnPhaseEnd();
    }

    protected virtual void OnPhaseStart() {

    }

    protected virtual void OnPhaseEnd() {
        Messenger.Broadcast(_callback);
    }

    protected abstract IEnumerator Run();

    //
    // Generic Methods for Phases
    //
    public virtual bool CanInputActions() {
        return false;
    }

    protected void ApplyStatusAilments(List<FightingEntity> entities) {
        foreach (var entity in entities) {
            entity.GetAilmentController().TickAilmentEffects(_phase);
        }
    }

    protected void DecrementStatusAilmentDuration(List<FightingEntity> entities) {
        foreach(var entity in entities) {
            entity.GetAilmentController().DecrementAllAilmentsDuration();
        }
    }
}