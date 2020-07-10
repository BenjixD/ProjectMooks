using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusAilment : ScriptableObject {
	public new string name;
	[Tooltip("Set to true if duration defaults to endless (this flag takes precedence over the defaultDuration).")]
	public bool infiniteDuration = false;
	[Tooltip("Duration used if the action inflicting this status ailment doesn't specify duration. Can leave this at 0.")]

	public int duration;
	public int level;
	[Tooltip("Phase to trigger Status Ailment")]
	public TurnPhase phase;

	public void SetDuration(int duration) {
		this.duration = duration;
	}

	public void SetInfiniteDuration() {
		infiniteDuration = true;
	}

	public virtual void DecrementDuration() {
		if (!infiniteDuration) {
			duration--;
		}
	}

	public abstract void StackWith(FightingEntity p, StatusAilment other);
	public abstract void ApplyTo(FightingEntity p);
	public abstract void Recover(FightingEntity p);
	public abstract void TickEffect(FightingEntity p);
}