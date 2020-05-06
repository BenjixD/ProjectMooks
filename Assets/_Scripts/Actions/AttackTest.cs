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
        int targetId = this.GetTargetIdFromString(splitCommand[1], user);
        if (targetId == -1) {
            Debug.Log("Target id -1");
            return false;
        }

        user.SetQueuedAction(new QueuedAction(user, this, new List<int>{ targetId } ));
        return true;
    }

    public override void ExecuteAction(FightingEntity user, List<FightingEntity> targets) {
        // TODO: calculate damage and inflict on targets

        int attackDamage = user.stats.GetPhysical();
        
        foreach (FightingEntity target in targets) {
            int defence = target.stats.GetDefense();
            int damage =  Mathf.Max(attackDamage - defence, 0);
            
            target.stats.SetHp(target.stats.GetHp() - damage);
        }
    }
}
