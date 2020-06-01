using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[System.Serializable]
public abstract class Phase {
    public TurnPhase _phase;

    protected TurnController _controller;
    protected string _callback;

    public Phase(TurnController controller, string callback) {
        _controller = controller;
        _callback = callback;
    }

    public IEnumerator RunPhase() {
        OnPhaseStart();
        yield return _controller.StartCoroutine(Run());
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
    protected void ApplyStatusAilments() {
        foreach (var entity in _controller.field.GetAllFightingEntities()) {
            entity.GetAilmentController().TickAilmentEffects(_phase);
        }
    }
}