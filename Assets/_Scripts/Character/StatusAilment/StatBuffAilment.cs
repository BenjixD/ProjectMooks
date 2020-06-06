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
		//TODO: Possible Animation Modifications
		switch(this.damageType) {
			case Type.PERCENTAGE:
				p.stats.SetPhysical(p.stats.GetPhysical() + (int)(p.stats.GetPhysical() * this.val / 100));
				break;
			case Type.FLAT:
				p.stats.SetPhysical(p.stats.GetPhysical() + (int)val);
				break;
		}
	}

	public override void Recover(FightingEntity p) {
		//TODO: Possible Animation Modifications
	}

	public override void TickEffect(FightingEntity p) {

	}
}