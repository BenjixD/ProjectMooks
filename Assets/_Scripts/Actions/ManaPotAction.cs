using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ManaPotAction", menuName = "Actions/Mana Pot", order = 2)]
public class ManaPotAction : ActionBase {
    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        // Fire One command format: !m fire [target number]
        if (!base.TryChooseAction(user, splitCommand)) {
            return false;
        }

        // Can only use mana pot on Hero
        user.SetQueuedAction(new QueuedAction(user, this, new List<int>{ 0 }));
        return true;
    }

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        PlayerStats before, after;
        List<DamageReceiver> receivers = new List<DamageReceiver>();
        int attackDamage = user.stats.special.GetValue();
        
        foreach (FightingEntity target in targets) {
            int defence = target.stats.resistance.GetValue();

            before = (PlayerStats)target.stats.Clone();
            target.stats.mana.ApplyDelta(attackDamage);
            after = (PlayerStats)target.stats.Clone();

            receivers.Add(new DamageReceiver(target, before, after));
        }

        return new FightResult(user, this, receivers);
    }
}
