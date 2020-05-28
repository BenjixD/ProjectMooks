using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireOne", menuName = "Actions/Fire One", order = 2)]
public class FireOneTest : ActionBase {
    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        if (!base.TryChooseAction(user, splitCommand)) {
            return false;
        }

        int targetId = this.GetTargetIdFromString(splitCommand[2], user);
        if (targetId == -1) {
            return false;
        }

        user.SetQueuedAction(new QueuedAction(user, this, new List<int>{ targetId }));
        return true;
    }

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        PlayerStats before, after;
        List<DamageReceiver> receivers = new List<DamageReceiver>();
        int attackDamage = user.stats.GetSpecial();
        
        foreach (FightingEntity target in targets) {
            int defence = target.stats.GetResistance();
            int damage =  Mathf.Max(attackDamage - defence, 0);

            before = (PlayerStats)target.stats.Clone();
            target.stats.SetHp(target.stats.GetHp() - damage);
            after = (PlayerStats)target.stats.Clone();

            receivers.Add(new DamageReceiver(target, before, after));
        }

        return new FightResult(user, this, receivers);
    }
}
