using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Defend", menuName = "Actions/Defend", order = 2)]
public class Defend : ActionBase {
    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        // Defend command format: !d
        if (!BasicValidation(splitCommand)) {
            return false;
        }

        TargetType targetType = user.isEnemy() ? TargetType.ENEMY : TargetType.PLAYER;
        user.SetQueuedAction(new QueuedAction(user, this, null, targetType));
        return true;
    }

    public override void ExecuteAction(FightingEntity user, List<FightingEntity> targets) {
        // TODO: defend
    }
}
