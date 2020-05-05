using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireOne", menuName = "Actions/Fire One", order = 2)]
public class FireOneTest : ActionBase {
    public override bool TryChooseAction(Player user, string[] splitCommand) {
        // Fire One command format: !m fire [target number]
        if (!BasicValidation(splitCommand)) {
            return false;
        }
        int targetId;
        if (!int.TryParse(splitCommand[2], out targetId)) {
            return false;
        }
        user.SetQueuedAction(new QueuedAction(this, new List<int>{ targetId }));
        return true;
    }

    public override void ExecuteAction(Player user, Player[] targets) {
        // TODO: calculate damage and inflict on targets
    }
}
