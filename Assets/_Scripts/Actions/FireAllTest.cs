using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireAll", menuName = "Actions/Fire All", order = 2)]
public class FireAllTest : ActionBase {
    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        // Fire All command format: !m fire2
        if (splitCommand.Length != 2) {
            Debug.Log("Malformed Fire All command. The proper Fire All command is !m fire2");
            return false;
        }
        // TODO: target all
        user.SetQueuedAction(new QueuedAction(this, null));
        return true;
    }

    public override void ExecuteAction(FightingEntity user, FightingEntity[] targets) {
        // TODO: calculate damage and inflict on targets
    }
}
