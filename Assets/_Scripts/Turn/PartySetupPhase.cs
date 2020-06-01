using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PartySetupPhase : Phase {

	protected BattleField _field {get; set;}

    public PartySetupPhase(BattleField field, TurnController controller, string callback) : base(controller, callback) {
    	this._field = field;
    }

    protected override void OnPhaseStart() {
    	this._field.RequestRecruitNewParty();
    }

    protected override IEnumerator Run() {
        this.ApplyStatusAilments();
        yield return null;
    }
}