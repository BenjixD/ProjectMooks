﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "Actions/Attack", order = 2)]
public class AttackTest : ActionBase {

    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        if (!base.TryChooseAction(user, splitCommand)) {
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

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        List<DamageReceiver> receivers = new List<DamageReceiver>();
        int attackDamage = user.stats.GetPhysical();
        
        foreach (FightingEntity target in targets) {
            PlayerStats before, after;
            int defence = target.stats.GetDefense();
            int damage =  Mathf.Max(attackDamage - defence, 0);

            before = (PlayerStats)target.stats.Clone();
            target.stats.SetHp(target.stats.GetHp() - damage);
            after = (PlayerStats)target.stats.Clone();

            receivers.Add(new DamageReceiver(target, before, after));
        }

        return new FightResult(user, this, receivers);
    }
}
