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
        Debug.LogError("Should not attempt to apply two Hero's Miracles");
	}

	public override void ApplyTo(Fighter p) {
		//TODO: Possible Animation Modifications
	}

	public override void Recover(Fighter p) {
		//TODO: Possible Animation Modifications
        if (p.fighter != null && GameManager.Instance != null) {
            GameManager.Instance.gameState.playerParty.EvictPlayer(p.index);
            if (p.fighter != null) {
                Destroy(p.fighter.gameObject);
            }

            Messenger.Broadcast(Messages.OnUpdateStatusBarsUI);    
        }
	}

	public override void TickEffect(Fighter p) {
        this.duration--;
        GameManager.Instance.battleComponents.ui.statusBarsUI.UpdateStatusBars();
        Messenger.Broadcast(Messages.OnUpdateStatusBarsUI);
	}
}
