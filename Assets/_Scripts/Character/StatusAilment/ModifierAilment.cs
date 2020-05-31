using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An ailment that is used only for its name
[CreateAssetMenu(fileName = "ModifierAilment", menuName = "StatusAilment/ModifierAilment")]
public class ModifierAilment : StatusAilment {

    // TODO: move this to somewhere more appropriate
    public const string MODIFIER_DEATH = "death";
    public const string MODIFIER_UNTARGETTABLE = "untargetable";
    public const string MODIFIER_CANNOT_USE_ACTION = "cannot move";

    public ModifierAilment SetName(string name) {
        this.name = name;
        return this;
    }

    public override void DecrementDuration() {
        // do not decrement
    }

	public override void ApplyTo(FightingEntity p) {
        this.duration = 31415926;
	}

    public override void StackWith(FightingEntity p, StatusAilment other)
    {
    }

    public override void Recover(FightingEntity p)
    {
    }

    public override void TickEffect(FightingEntity p)
    {
    }
}