using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireOne", menuName = "Actions/Fire One", order = 2)]
public class FireOneTest : ActionBase {
    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        // Fire One command format: !m fire [target number]
        if (!BasicValidation(splitCommand)) {
            return false;
        }
        int targetId;
        if (!int.TryParse(splitCommand[2], out targetId)) {
            return false;
        }

        user.SetQueuedAction(new QueuedAction(user, this, new List<int>{ targetId }));
        return true;
    }

    public override void ExecuteAction(FightingEntity user, List<FightingEntity> targets) {
        int attackDamage = user.stats.GetSpecial();
        
        foreach (FightingEntity target in targets) {
            int defence = target.stats.GetResistance();
            int damage =  Mathf.Max(attackDamage - defence, 0);
            
            target.stats.SetHp(target.stats.GetHp() - damage);
        }
    }
}
