using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Heal", menuName = "Actions/Heal", order = 2)]
public class Heal : ActionBase {
    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        if (!base.TryChooseAction(user, splitCommand)) {
            return false;
        }

        int targetId = this.GetTargetIdFromString(splitCommand[1], user);
        if (targetId == -1) {
            return false;
        }

        user.SetQueuedAction(new QueuedAction(user, this, new List<int>{ targetId }));
        return true;
    }
}