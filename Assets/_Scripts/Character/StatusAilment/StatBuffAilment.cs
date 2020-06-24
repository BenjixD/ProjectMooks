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

	public StatModifier.Type damageType = StatModifier.Type.ADD_PERCENTAGE;
	public float val;

    public StatType statType;

    private List<StatModifier> modifiers = new List<StatModifier>();

	public override void StackWith(FightingEntity p, StatusAilment other) {
        StatBuffAilment otherAilment = (StatBuffAilment) other;
        this.ApplyHelper(p, otherAilment);
	}

	public override void ApplyTo(FightingEntity p) {
        this.ApplyHelper(p, this);
	}

	public override void Recover(FightingEntity p) {
		//TODO: Possible Animation Modifications
        foreach (StatModifier modifier in this.modifiers) {
            p.stats.physical.RemoveModifier(modifier);
        }
	}

	public override void TickEffect(FightingEntity p) {

	}

    private void ApplyHelper(FightingEntity p, StatBuffAilment ailment) {
        modifiers.Add(p.stats.physical.ApplyDeltaModifier(ailment.val, ailment.damageType));
    }

}