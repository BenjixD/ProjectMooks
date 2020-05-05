using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Actions/Attack", order = 2)]
public class AttackTest : ActionBase {
    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        // Attack command format: !a [target number]
        if (splitCommand.Length != 2) {
            Debug.Log("Malformed attack command. The proper attack command is !a [target number]");
            return false;
        }
        int targetId;
        if (!int.TryParse(splitCommand[1], out targetId)) {
            Debug.Log("Malformed attack command. " + splitCommand[1] + " is not a valid target");
            return false;
        }
        user.SetQueuedAction(new QueuedAction(this, new List<int>{ targetId }));
        return true;
    }

    public override void ExecuteAction(FightingEntity user, FightingEntity[] targets) {
        // TODO: calculate damage and inflict on targets
    }
}
