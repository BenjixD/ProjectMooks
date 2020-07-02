using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OutOfJuiceAilment", menuName = "StatusAilment/Out of Juice")]
public class OutOfJuiceAilment : StatusAilment {
	public override void StackWith(FightingEntity p, StatusAilment other) {
		// Doesn't stack
        Debug.LogError("Should not attempt to apply two Hero's Miracles");
	}

	public override void ApplyTo(FightingEntity p) {
		//TODO: Possible Animation Modifications
	}

	public override void Recover(FightingEntity p) {
		//TODO: Possible Animation Modifications

        
        GameManager.Instance.battleComponents.EvictMook(p);
	}

	public override void TickEffect(FightingEntity p) {
        p.stats.mana.ApplyDelta(-1);
        GameManager.Instance.battleComponents.ui.statusBarsUI.UpdateStatusBars();

	}
}
