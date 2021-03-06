using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Add other stats, use in Player...

[CreateAssetMenu(fileName = "StatBuffAilment", menuName = "StatusAilment/StatBuffAilment")]
public class StatBuffAilment : StatusAilment {

	public StatModifier.Type damageType = StatModifier.Type.ADD_PERCENTAGE;
	public float val;

    public ModifiableStat statType;

    private List<StatModifier> modifiers = new List<StatModifier>();

	public override void StackWith(Fighter p, StatusAilment other) {
        StatBuffAilment otherAilment = (StatBuffAilment) other;
        this.ApplyHelper(p, otherAilment);
	}

	public override void ApplyTo(Fighter p) {
        if (!p.stats.GetModifiableStats().ContainsKey(statType)) {
            return;
        }

        this.ApplyHelper(p, this);
	}

	public override void Recover(Fighter p) {
		//TODO: Possible Animation Modifications
        foreach (StatModifier modifier in this.modifiers) {
            p.stats.GetModifiableStats()[statType].RemoveModifier(modifier);
        }

        modifiers.Clear();
	}

	public override void TickEffect(Fighter p) {

	}

    private void ApplyHelper(Fighter p, StatBuffAilment ailment) {
        modifiers.Add(p.stats.GetModifiableStats()[ailment.statType].ApplyDeltaModifier(ailment.val, ailment.damageType));
    }

}