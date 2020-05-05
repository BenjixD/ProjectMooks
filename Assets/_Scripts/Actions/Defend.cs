﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Defend", menuName = "Actions/Defend", order = 2)]
public class Defend : ActionBase {
    public override bool TryChooseAction(Player user, string[] splitCommand) {
        // Defend command format: !d
        if (!BasicValidation(splitCommand)) {
            return false;
        }
        user.SetQueuedAction(new QueuedAction(this, null));
        return true;
    }

    public override void ExecuteAction(Player user, Player[] targets) {
        // TODO: defend
    }
}
