using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageOverTimeStatusAilment", menuName = "StatusAilment/Damage Over Time")]
public class DamageOverTimeStatusAilment : StatusAilment {
	public enum Type {
		PERCENTAGE,
		FLAT
	}

	[Tooltip("Damage Type")]
	public Type damageType;
	[Tooltip("[0f, 1f] Percentage OR [0f,) Flat truncated to whole number")]
	public float val;

	public override void StackWith(Fighter p, StatusAilment other) {
		if(this.name == other.name) {
			this.level = (int)Mathf.Max(this.level, other.level);
			this.duration = (int)Mathf.Max(this.duration, other.duration);
		} else {
			Debug.LogError("Attempting to stack " + this.name + " with " + other.name);
		}
	}

	public override void ApplyTo(Fighter p) {
		//TODO: Possible Animation Modifications
	}

	public override void Recover(Fighter p) {
		//TODO: Possible Animation Modifications
	}

	public override void TickEffect(Fighter p) {
		int damage = 0;
		switch(this.damageType) {
			case Type.PERCENTAGE:
				damage = (int)(p.stats.hp.GetValue() * this.val);
				// Minimum % damage is 1
				damage = Mathf.Max(damage, 1);
				break;
			case Type.FLAT:
				damage = (int)val;
				break;
		}
		Debug.Log("Taking Damage Over Time: " + damage);
        p.stats.hp.ApplyDelta(-damage);
	}
}