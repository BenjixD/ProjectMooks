using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionAnimation", menuName = "ActionsAnimations/ExplosionAnimation", order = 0)]
public class ExplosionAnimation : ActionAnimation {
    public override IEnumerator SlideIn(FightingEntity user, List<FightingEntity> targets) {
        // Slide toward enemies despite Explosion targeting all fighters
        List<FightingEntity> enemyTargets = targets.Filter(target => target.isEnemy());
        yield return GameManager.Instance.time.GetController().StartCoroutine(base.SlideIn(user, enemyTargets));
    }
}
