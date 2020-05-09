using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Defend", menuName = "Actions/Defend", order = 2)]
public class Defend : ActionBase {
    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        // Defend command format: !d
        if (!BasicValidation(splitCommand)) {
            return false;
        }

        user.SetQueuedAction(new QueuedAction(user, this, null));
        return true;
    }

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        user.modifiers.Add("defend");
        return new FightResult(user, this);
    }
}
