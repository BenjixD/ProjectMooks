using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusAilment : ScriptableObject {
	public new string name;
	[Tooltip("Duration used if the action inflicting this status ailment doesn't specify duration. Can leave this at 0.")]
	public int defaultDuration;
	[HideInInspector] public int duration;
	public int level;
	[Tooltip("Phase to trigger Status Ailment")]
	public TurnPhase phase;

	private void Awake() {
		// Set duration to default if one is not set
		if (duration == 0 && defaultDuration != 0) {
			duration = defaultDuration;
		}
	}

	public void SetDuration(int duration) {
		this.duration = duration;
	}

	public virtual void DecrementDuration() {
		duration--;
	}

	public abstract void StackWith(FightingEntity p, StatusAilment other);
	public abstract void ApplyTo(FightingEntity p);
	public abstract void Recover(FightingEntity p);
	public abstract void TickEffect(FightingEntity p);
}