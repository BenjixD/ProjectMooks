using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fire", menuName = "Actions/Fire", order = 2)]
public class Fire : ActionBase {
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
