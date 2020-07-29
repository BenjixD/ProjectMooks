using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AilmentType{
   PERSISTENT,
   DESTROY_ON_BATTLE_END 
};
public abstract class StatusAilment : ScriptableObject {
	public new string name;
	public Sprite icon;
	[Tooltip("Set to true if duration defaults to endless (this flag takes precedence over the defaultDuration).")]
	public bool infiniteDuration = false;
	[Tooltip("Duration used if the action inflicting this status ailment doesn't specify duration. Can leave this at 0.")]
	public int duration;
	public int level;
	[Tooltip("Phase to trigger Status Ailment")]
	public TurnPhase phase;

    public AilmentType ailmentType = AilmentType.DESTROY_ON_BATTLE_END;

    public bool isDestroying = false;

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

	public abstract void StackWith(Fighter p, StatusAilment other);
	public abstract void ApplyTo(Fighter p);
	public abstract void Recover(Fighter p);
	public abstract void TickEffect(Fighter p);
}