using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionAnimation", menuName = "ActionsAnimations/ExplosionAnimation", order = 0)]
public class ExplosionAnimation : ActionAnimation {
    protected override void SetSlidePositions(FightingEntity user, List<FightingEntity> targets, ref Vector3 userDestination) {
        // Slide toward enemies despite Explosion targeting all fighters
        List<FightingEntity> enemyTargets = targets.Filter(target => target.isEnemy());
        base.SetSlidePositions(user, enemyTargets, ref userDestination);
    }
}
