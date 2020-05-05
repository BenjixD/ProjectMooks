using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAllTest : ActionBase {
    public FireAllTest() {
        actionType = ActionType.FIRE_ALL;
    }

    public override bool TryChooseAction(Player user, string[] splitCommand) {
        // Fire All command format: !m fire2
        if (splitCommand.Length != 2) {
            Debug.Log("Malformed Fire All command. The proper Fire All command is !m fire2");
            return false;
        }
        // TODO: target all
        user.SetQueuedAction(new QueuedAction(this, null));
        return true;
    }

    public override void ExecuteAction(Player user, Player[] targets) {
        // TODO: calculate damage and inflict on targets
    }
}
