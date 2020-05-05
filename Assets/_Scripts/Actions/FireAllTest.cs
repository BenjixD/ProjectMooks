using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireAll", menuName = "Actions/Fire All", order = 2)]
public class FireAllTest : ActionBase {
    public override bool TryChooseAction(Player user, string[] splitCommand) {
        // Fire All command format: !m fire2
        if (!BasicValidation(splitCommand)) {
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
