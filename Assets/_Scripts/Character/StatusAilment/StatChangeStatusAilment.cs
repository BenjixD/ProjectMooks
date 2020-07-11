using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatChangeStatusAilment", menuName = "StatusAilment/Stat Change")]
public class StatChangeStatusAilment : StatusAilment {
	public enum Type {
		PERCENTAGE,
		FLAT
	}

	[Tooltip("Stat to change.")]
	public Stat stat;
	[Tooltip("Bonus Type")]
	public Type type;
	[Tooltip("[0,1] Percentage Bonus, or [0,) Flat Bonus")]
	public float modifier;
	private int _flatChange;


    private PlayerStatWithModifiers playerStat;
    private StatModifier statModifier;

	public override void StackWith(Fighter p, StatusAilment other) {
		if(this.name == other.name) {
			this.level = (int)Mathf.Max(this.level, other.level);
			this.duration = (int)Mathf.Max(this.duration, other.duration);
		} else {
			Debug.LogError("Attempting to stack " + this.name + " with " + other.name);
		}
	}

	public override void ApplyTo(Fighter p) {
        PlayerStat tmpStat = p.stats.GetStat(stat);
        if (tmpStat.GetType() != typeof(PlayerStatWithModifiers)) {
            Debug.LogError("Error: Should be stat with modifier");
        } else {
            this.playerStat = (PlayerStatWithModifiers)p.stats.GetStat(stat);
        }

		//TODO: Possible Animation Modifications
		switch(type) {
			case Type.PERCENTAGE:
				_flatChange = (int)Mathf.Ceil(playerStat.GetValue() * modifier);
                this.statModifier = playerStat.ApplyDeltaModifier(_flatChange, StatModifier.Type.FLAT);
				break;
			case Type.FLAT:
				_flatChange = (int)modifier;
                this.statModifier = playerStat.ApplyDeltaModifier(_flatChange, StatModifier.Type.ADD_PERCENTAGE);
				break;
		}
		Debug.Log("Defending: " + _flatChange + " damage");
	}

	public override void Recover(Fighter p) {
		//TODO: Possible Animation Modifications
		Debug.Log("Recovering: " + _flatChange + " " + stat);
        this.playerStat.RemoveModifier(this.statModifier);
	}

	public override void TickEffect(Fighter p) {

	}
}