using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ParalyzeStatusAilment", menuName = "StatusAilment/Paralyze")]
public class ParalyzeStatusAilment : StatusAilment {
	[Header("Chance to Trigger Paralysis")]
	public float chance; 
	[Header("Replaced Action")]
	public ActionBase action;

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
        if (p.fighter == null) {
            Debug.LogError("ERROR: Fighter is null!");
        }

		float roll = Random.Range(0f, 1f);
		if(roll <= chance) {
			p.fighter.SetQueuedAction(action, new List<int>(){p.index});	
		}
	}
}