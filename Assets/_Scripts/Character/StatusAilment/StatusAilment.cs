using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusAilment : ScriptableObject {
	public new string name;
	[Tooltip("Duration used if the action inflicting this status ailment doesn't specify duration. Can leave this at 0.")]
	public int defaultDuration;
	[Tooltip("Set to true if duration defaults to endless (this flag takes precedence over the defaultDuration).")]
	public bool infiniteDefaultDuration;
	private bool _infiniteDuration = false;
	[HideInInspector] public int duration;
	public int level;
	[Tooltip("Phase to trigger Status Ailment")]
	public TurnPhase phase;

	private void Awake() {
		// Set duration to default if one is not set
		if (duration == 0) {
			if (infiniteDefaultDuration) {
				_infiniteDuration = true;
			} else if (defaultDuration != 0) {
				duration = defaultDuration;
			}
		}
	}

	public void SetDuration(int duration) {
		this.duration = duration;
	}

	public void SetInfiniteDuration() {
		_infiniteDuration = true;
	}

	public virtual void DecrementDuration() {
		if (!_infiniteDuration) {
			duration--;
		}
	}

	public abstract void StackWith(FightingEntity p, StatusAilment other);
	public abstract void ApplyTo(FightingEntity p);
	public abstract void Recover(FightingEntity p);
	public abstract void TickEffect(FightingEntity p);
}