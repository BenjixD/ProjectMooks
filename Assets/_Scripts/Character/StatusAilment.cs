using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusAilment {
	public string name;
	public int duration;
	public int level;

	public virtual void DecrementDuration() {
		duration--;
	}

	public abstract void StackWith(Player p, StatusAilment other);
	public abstract void ApplyTo(Player p);
	public abstract void Recover(Player p);
	public abstract void TickEffect(Player p);
}