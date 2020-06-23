using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HerosMiracleAilment", menuName = "StatusAilment/Hero's Miracle")]
public class HerosMiracleAilment : StatusAilment {

    [SerializeField] private NothingAction nothingAction = null;
    [SerializeField] private int reviveHP = 1;

	public override void StackWith(FightingEntity p, StatusAilment other) {
		// Doesn't stack
        Debug.LogError("Should not attempt to apply two Hero's Miracles");
	}

	public override void ApplyTo(FightingEntity p) {
		//TODO: Possible Animation Modifications
	}

	public override void Recover(FightingEntity p) {
		//TODO: Possible Animation Modifications
	}

	public override void TickEffect(FightingEntity p) {

        if (this.duration <= 1) {
            p.RemoveModifier(ModifierAilment.MODIFIER_CANNOT_USE_ACTION);
            p.RemoveModifier(ModifierAilment.MODIFIER_DEATH);
            p.RemoveModifier(ModifierAilment.MODIFIER_UNTARGETTABLE);
            //TODO: Play Animation
            // Temp animation will be to just set gameObject active 
            p.stats.hp.ApplyDelta(reviveHP);
            p.gameObject.SetActive(true);
        } else {
            p.SetQueuedAction(new QueuedAction(p, nothingAction, new List<int>{ p.targetId }  ));	
        }

	}
}
