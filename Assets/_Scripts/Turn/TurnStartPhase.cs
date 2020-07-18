using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class TurnStartPhase : Phase {
    protected BattleField _field {get; set;}
    protected BattleUI _ui {get; set;}

    public TurnStartPhase(BattleField field, BattleUI ui, string callback) : base(TurnPhase.TURN_START, callback) {
        this._field = field;
        this._ui = ui;
    }

    protected override IEnumerator Run() {
        this.ApplyStatusAilments(this._field.GetAllFightingEntities());
        yield return GameManager.Instance.time.GetController().StartCoroutine(_ui.roundStartUI.PlayAnimation());
        yield return null;
    }
}