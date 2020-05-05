using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireAll", menuName = "Actions/Fire All", order = 2)]
public class FireAllTest : ActionBase {
    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        // Fire All command format: !m fire2
        if (!BasicValidation(splitCommand)) {
            return false;
        }
        // TODO: target all
        TargetType targetType = user.isEnemy() ? TargetType.PLAYER : TargetType.ENEMY;
        user.SetQueuedAction(new QueuedAction(user, this, null, targetType));
        return true;
    }

    public override void ExecuteAction(FightingEntity user, List<FightingEntity> targets) {
        // TODO: calculate damage and inflict on targets
    }
}
