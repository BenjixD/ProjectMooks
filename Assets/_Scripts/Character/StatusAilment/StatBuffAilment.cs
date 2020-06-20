using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Add other stats, use in Player...
[System.Serializable]
public enum StatType {
    PHYSICAL
}
[CreateAssetMenu(fileName = "StatBuffAilment", menuName = "StatusAilment/StatBuffAilment")]
public class StatBuffAilment : StatusAilment {
	public enum Type {
		PERCENTAGE,
		FLAT
	}


	public Type damageType;
	public float val;

    public StatType statType;

	public override void StackWith(FightingEntity p, StatusAilment other) {
		
	}

	public override void ApplyTo(FightingEntity p) {
		switch(this.damageType) {
			case Type.PERCENTAGE:
                p.stats.physical.ApplyDeltaModifier(this.val, StatModifier.Type.ADD_PERCENTAGE);
				break;
			case Type.FLAT:
				p.stats.physical.ApplyDeltaModifier(this.val, StatModifier.Type.FLAT);
				break;
		}
	}

	public override void Recover(FightingEntity p) {
		//TODO: Possible Animation Modifications
	}

	public override void TickEffect(FightingEntity p) {

	}
}