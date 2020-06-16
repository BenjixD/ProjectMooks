using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatChangeStatusAilment", menuName = "StatusAilment/Stat Change")]
public class StatChangeStatusAilment : StatusAilment {
	public enum Type {
		PERCENTAGE,
		FLAT
	}

	[Tooltip("Stat to change.")]
	public Stat stat;
	[Tooltip("Bonus Type")]
	public Type type;
	[Tooltip("[0,1] Percentage Bonus, or [0,) Flat Bonus")]
	public float modifier;
	private int _flatChange;

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
				_flatChange = (int)Mathf.Ceil(p.stats.GetStat(stat) * modifier);
				p.stats.ModifyStat(stat, _flatChange);
				break;
			case Type.FLAT:
				_flatChange = (int)modifier;
				p.stats.ModifyStat(stat, _flatChange);
				break;
		}
		Debug.Log("Defending: " + _flatChange + " damage");
	}

	public override void Recover(FightingEntity p) {
		//TODO: Possible Animation Modifications
		Debug.Log("Recovering: " + _flatChange + " " + stat);
		p.stats.ModifyStat(stat, -_flatChange);
	}

	public override void TickEffect(FightingEntity p) {

	}
}