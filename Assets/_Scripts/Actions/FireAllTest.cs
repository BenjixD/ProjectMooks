﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireAll", menuName = "Actions/Fire All", order = 2)]
public class FireAllTest : ActionBase {
    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        // Fire All command format: !m fire2
        if (!BasicValidation(splitCommand)) {
            return false;
        }
        List<int> targetIds = new List<int>();
        for (int i = 0; i < GameManager.Instance.battleController.enemies.Count; i++) {
            targetIds.Add(i);
        }

        user.SetQueuedAction(new QueuedAction(user, this, targetIds));
        return true;
    }
    
    public override void ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        int attackDamage = user.stats.GetSpecial();
        
        foreach (FightingEntity target in targets) {
            int defence = target.stats.GetResistance();
            int damage =  Mathf.Max(attackDamage - defence, 0);
            
            target.stats.SetHp(target.stats.GetHp() - damage);
        }
    }
}
