using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefenseStatusAilment", menuName = "StatusAilment/Defense")]
public class DefenseStatusAilment : StatusAilment {
	public enum Type {
		PERCENTAGE,
		FLAT
	}

	[Header("Defense Bonus Type")]
	public Type type;
	[Header("[0,1] Percentage Bonus, or [0,) Flat Bonus")]
	public float defenseModifier;
	private int _defenseGrowth;

	public override void StackWith(FightingEntity p, StatusAilment other) {
		if(this.name == other.name) {
			this.level = (int)Mathf.Max(this.level, other.level);
			this.duration = (int)Mathf.Max(this.duration, other.duration);
		} else {
			Debug.LogError("Attempting to stack " + this.name + " with " + other.name);
		}
	}

	public override void ApplyTo(FightingEntity p) {
		//TODO: Possible Animation Modifications
		switch(type) {
			case Type.PERCENTAGE:
				_defenseGrowth = (int)Mathf.Ceil(p.stats.GetDefense() * defenseModifier);
				p.stats.SetDefense(p.stats.GetDefense() + _defenseGrowth);
				break;
			case Type.FLAT:
				_defenseGrowth = (int)defenseModifier;
				p.stats.SetDefense(p.stats.GetDefense() + _defenseGrowth);
				break;
		}
		Debug.Log("Defending: " + _defenseGrowth + " damage");
	}

	public override void Recover(FightingEntity p) {
		//TODO: Possible Animation Modifications
		p.stats.SetDefense(p.stats.GetDefense() - _defenseGrowth);
	}

	public override void TickEffect(FightingEntity p) {

	}
}