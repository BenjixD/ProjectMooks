using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ParalyzeStatusAilment", menuName = "StatusAilment/Paralyze")]
public class ParalyzeStatusAilment : StatusAilment {
	[Header("Chance to Trigger Paralysis")]
	public float chance; 
	[Header("Replaced Action")]
	public ActionBase action;

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
		float roll = Random.Range(0f, 1f);
		if(roll <= chance) {
			Debug.Log("PARALYZING");
			p.SetQueuedAction(new QueuedAction(p, action, new List<int>(){p.targetId}));	
		}
	}
}