using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class TurnEndPhase : Phase {
    protected BattleField _field {get; set;}

    public TurnEndPhase(BattleField field, string callback) : base(TurnPhase.TURN_END, callback) {
        this._field = field;
    }

    protected override IEnumerator Run() {
        this.ApplyStatusAilments(this._field.GetAllFightingEntities());
        this.DecrementStatusAilmentDuration(this._field.GetAllFightingEntities());
        yield return null;
    }
}