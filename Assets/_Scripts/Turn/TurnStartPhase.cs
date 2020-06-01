using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class TurnStartPhase : Phase {

    public TurnStartPhase(TurnController controller, string callback) : base(controller, callback) {}

    protected override IEnumerator Run() {
        this.ApplyStatusAilments();
        yield return null;
    }
}