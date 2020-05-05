using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Actions/Attack", order = 2)]
public class AttackTest : ActionBase {
    public override bool TryChooseAction(Player user, string[] splitCommand) {
        // Attack command format: !a [target number]
        if (!BasicValidation(splitCommand)) {
            return false;
        }
        int targetId;
        if (!int.TryParse(splitCommand[1], out targetId)) {
            return false;
        }
        user.SetQueuedAction(new QueuedAction(this, new List<int>{ targetId }));
        return true;
    }

    public override void ExecuteAction(Player user, Player[] targets) {
        // TODO: calculate damage and inflict on targets
    }
}
