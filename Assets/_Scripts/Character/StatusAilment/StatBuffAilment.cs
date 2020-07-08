using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Add other stats, use in Player...

[CreateAssetMenu(fileName = "StatBuffAilment", menuName = "StatusAilment/StatBuffAilment")]
public class StatBuffAilment : StatusAilment {

	public StatModifier.Type damageType = StatModifier.Type.ADD_PERCENTAGE;
	public float val;

    public Stat statType;

    private List<StatModifier> modifiers = new List<StatModifier>();

	public override void StackWith(FightingEntity p, StatusAilment other) {
        StatBuffAilment otherAilment = (StatBuffAilment) other;
        this.ApplyHelper(p, otherAilment);
	}

	public override void ApplyTo(FightingEntity p) {
        if (!p.stats.GetModifiableStats().ContainsKey(statType)) {
            return;
        }

        this.ApplyHelper(p, this);
	}

	public override void Recover(FightingEntity p) {
		//TODO: Possible Animation Modifications
        foreach (StatModifier modifier in this.modifiers) {
            p.stats.GetModifiableStats()[statType].RemoveModifier(modifier);
        }

        modifiers.Clear();
	}

	public override void TickEffect(FightingEntity p) {

	}

    private void ApplyHelper(FightingEntity p, StatBuffAilment ailment) {
        Debug.Log(ailment.statType);
        Debug.Log("Before: " + p.stats.GetModifiableStats()[ailment.statType].GetValue());
        modifiers.Add(p.stats.GetModifiableStats()[ailment.statType].ApplyDeltaModifier(ailment.val, ailment.damageType));
        Debug.Log("After: " + p.stats.GetModifiableStats()[ailment.statType].GetValue());
    }

}