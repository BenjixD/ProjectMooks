using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewAction", menuName = "Actions/DoubleAttack", order = 3)]
public class DoubleAttackAction : ActionBase {
    public override IEnumerator ExecuteAction(FightingEntity user, List<FightingEntity> targets, BattleFight battleFight) {
        yield return GameManager.Instance.time.GetController().StartCoroutine(base.ExecuteAction(user, targets, battleFight));
        yield return GameManager.Instance.time.GetController().StartCoroutine(base.ExecuteAction(user, targets, battleFight));
    }
}
