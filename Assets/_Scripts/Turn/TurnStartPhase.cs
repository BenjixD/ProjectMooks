using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class TurnStartPhase : Phase {
    protected BattleField _field {get; set;}

    public TurnStartPhase(BattleField field, string callback) : base(TurnPhase.TURN_START, callback) {
        this._field = field;
    }

    protected override IEnumerator Run() {
        this.ApplyStatusAilments(this._field.GetAllFightingEntities());
        yield return null;
    }
}