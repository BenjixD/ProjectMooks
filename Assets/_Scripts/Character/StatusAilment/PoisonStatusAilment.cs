using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoisonStatusAilment", menuName = "StatusAilment/Poison")]
public class PoisonStatusAilment : StatusAilment {
	public enum Type {
		PERCENTAGE,
		FLAT
	}
	[Header("Poison Damage Type")]
	public Type damageType;
	[Header("[0f, 1f] Percentage OR [0f,) Flat truncated to whole number")]
	public float val;

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
	}

	public override void Recover(FightingEntity p) {
		//TODO: Possible Animation Modifications
	}

	public override void TickEffect(FightingEntity p) {
		Debug.Log("Taking Poison Damage");
		switch(this.damageType) {
			case Type.PERCENTAGE:
				p.stats.SetHp(p.stats.GetHp() - (int)(p.stats.GetHp() * this.val / 100));
				break;
			case Type.FLAT:
				p.stats.SetHp(p.stats.GetHp() - (int)val);
				break;
		}
	}
}