using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusAilment : ScriptableObject {
	public string name;
	public int duration;
	public int level;
	[Header("Phase to trigger Status Ailment")]
	public BattlePhase phase;

	public virtual void DecrementDuration() {
		duration--;
	}

	public abstract void StackWith(FightingEntity p, StatusAilment other);
	public abstract void ApplyTo(FightingEntity p);
	public abstract void Recover(FightingEntity p);
	public abstract void TickEffect(FightingEntity p);
}