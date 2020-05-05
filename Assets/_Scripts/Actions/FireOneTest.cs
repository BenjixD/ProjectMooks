using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireOne", menuName = "Actions/Fire One")]
public class FireOneTest : ActionBase {
    public override bool TryChooseAction(Player user, string[] splitCommand) {
        // Fire One command format: !m fire [target number]
        if (splitCommand.Length != 3) {
            Debug.Log("Malformed Fire One command. The proper Fire One command is !m fire1 [target number]");
            return false;
        }
        int targetId;
        if (!int.TryParse(splitCommand[2], out targetId)) {
            Debug.Log("Malformed magic command. " + splitCommand[2] + " is not a valid target");
            return false;
        }
        user.SetQueuedAction(new QueuedAction(this, new List<int>{ targetId }));
        return true;
    }

    public override void ExecuteAction(Player user, Player[] targets) {
        // TODO: calculate damage and inflict on targets
    }
}
