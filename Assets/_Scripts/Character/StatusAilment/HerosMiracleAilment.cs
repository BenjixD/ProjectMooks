using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HerosMiracleAilment", menuName = "StatusAilment/Hero's Miracle")]
public class HerosMiracleAilment : StatusAilment {

    [SerializeField] private NothingAction nothingAction = null;
    [SerializeField] private int reviveHP = 1;

	public override void StackWith(Fighter p, StatusAilment other) {
		// Doesn't stack
        Debug.LogError("Should not attempt to apply two Hero's Miracles");
	}

	public override void ApplyTo(Fighter p) {
		//TODO: Possible Animation Modifications
	}

	public override void Recover(Fighter p) {
		//TODO: Possible Animation Modifications
	}

	public override void TickEffect(Fighter p) {
        if (p.fighter == null) {
            Debug.LogError("ERROR: FightingEntity is null");
        }

        if (this.duration <= 1) {
            p.fighter.RemoveModifier(ModifierAilment.MODIFIER_CANNOT_USE_ACTION);
            p.fighter.RemoveModifier(ModifierAilment.MODIFIER_DEATH);
            p.fighter.RemoveModifier(ModifierAilment.MODIFIER_UNTARGETTABLE);
            //TODO: Play Animation
            // Temp animation will be to just set gameObject active 
            p.stats.hp.ApplyDelta(reviveHP);
            p.fighter.gameObject.SetActive(true);
        } else {
            p.fighter.SetQueuedAction(new QueuedAction(p.fighter, nothingAction, new List<int>{ p.index }  ));	
        }

	}
}
