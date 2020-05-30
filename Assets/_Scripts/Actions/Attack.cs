using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Actions/Attack", order = 2)]
public class Attack : ActionBase {
    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        if (!base.TryChooseAction(user, splitCommand)) {
            return false;
        }
        
        int targetId = this.GetTargetIdFromString(splitCommand[1], user);
        if (targetId == -1) {
            Debug.Log("Target id -1");
            return false;
        }

        user.SetQueuedAction(new QueuedAction(user, this, new List<int>{ targetId } ));
        return true;
    }
}
