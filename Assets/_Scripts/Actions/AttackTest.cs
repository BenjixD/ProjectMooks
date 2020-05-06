using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Actions/Attack", order = 2)]
public class AttackTest : ActionBase {
    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        // Attack command format: !a [target number]
        if (!BasicValidation(splitCommand)) {
            return false;
        }
        int targetId;
        if (!int.TryParse(splitCommand[1], out targetId)) {
            return false;
        }

        TargetType targetType = user.isEnemy() ? TargetType.PLAYER : TargetType.ENEMY;
        user.SetQueuedAction(new QueuedAction(user, this, new List<int>{ targetId }, targetType ));
        return true;
    }

    public override void ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        // TODO: calculate damage and inflict on targets
    }
}
