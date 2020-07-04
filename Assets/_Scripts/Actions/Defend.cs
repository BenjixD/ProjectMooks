﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Defend", menuName = "Actions/Defend", order = 5)]
public class Defend : ActionBase {
    public override bool QueueAction(FightingEntity user, string[] splitCommand) {
        user.SetQueuedAction(new QueuedAction(user, this, new List<int>{ user.targetId }));
        return true;
    }

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        InflictStatuses(user);
        return new FightResult(user, this);
    }

    protected override void Animate(AnimationController controller) {
        controller.AddToTrack("idle", "defend start", false, 0);
    }
}
