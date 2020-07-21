using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OutOfJuiceAilment", menuName = "StatusAilment/Out of Juice")]
public class OutOfJuiceAilment : StatusAilment {

	public override void DecrementDuration() {
        // Want to decrement manually on tick
	}

	public override void StackWith(Fighter p, StatusAilment other) {
		// Doesn't stack
        Debug.LogError("Should not attempt to apply two of these");
	}

	public override void ApplyTo(Fighter p) {
		//TODO: Possible Animation Modifications
	}

	public override void Recover(Fighter p) {
		//TODO: Possible Animation Modifications
        if (p.fighter != null && GameManager.Instance != null) {

            p.stats.hp.SetValue(0);
            Messenger.Broadcast<DeathResult>(Messages.OnEntityDeath, new DeathResult(p.fighter, new DamageReceiver(p.fighter, null, null), null));
        }
	}

	public override void TickEffect(Fighter p) {
        this.duration--;
        GameManager.Instance.battleComponents.ui.statusBarsUI.UpdateStatusBars();
        Messenger.Broadcast(Messages.OnUpdateStatusBarsUI);
	}
}
