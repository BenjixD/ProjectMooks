using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class PartySetupPhase : Phase {

    protected BattleField _field {get; set;}

    public PartySetupPhase(BattleField field, string callback) : base(TurnPhase.PARTY_SETUP, callback) {
        this._field = field;
    }

    protected override void OnPhaseStart() {
        PlayerObject heroPlayer = this._field.GetHeroPlayer();
        if (!heroPlayer.HasModifier(ModifierAilment.MODIFIER_DEATH)) {
            this._field.RequestRecruitNewParty();
        }
    }

    protected override IEnumerator Run() {
        this.ApplyStatusAilments(this._field.GetAllFightingEntities());
        yield return null;
    }
}