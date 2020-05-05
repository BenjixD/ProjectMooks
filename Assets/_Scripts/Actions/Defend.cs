﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Defend", menuName = "Actions/Defend", order = 2)]
public class Defend : ActionBase {
    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        // Defend command format: !d
        if (splitCommand.Length != 1) {
            Debug.Log("Malformed defend command. The proper defend command is !d");
            return false;
        }
        user.SetQueuedAction(new QueuedAction(this, null));
        return true;
    }

    public override void ExecuteAction(FightingEntity user, FightingEntity[] targets) {
        // TODO: defend
    }
}
