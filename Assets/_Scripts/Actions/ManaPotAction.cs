using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ManaPotAction", menuName = "Actions/Mana Pot", order = 2)]
public class ManaPotAction : ActionBase {
    public override bool QueueAction(FightingEntity user, string[] splitCommand) {
        // Can only use mana pot on Hero
        user.SetQueuedAction(new QueuedAction(user, this, new List<int>{ 0 }));
        return true;
    }

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        PlayerStats before, after;
        List<DamageReceiver> receivers = new List<DamageReceiver>();
        int attackDamage = user.stats.special.GetValue();
        
        foreach (FightingEntity target in targets) {
            InstantiateDamagePopup(target, attackDamage);
            before = (PlayerStats)target.stats.Clone();
            target.stats.mana.ApplyDelta(attackDamage);
            after = (PlayerStats)target.stats.Clone();

            receivers.Add(new DamageReceiver(target, before, after));
        }

        return new FightResult(user, this, receivers);
    }

    protected override void InstantiateDamagePopup(FightingEntity target, int damage) {
        DamagePopup popup = Instantiate(damagePopupCanvasPrefab).GetComponent<DamagePopup>();
        popup.Initialize(target, damage, DamageType.MANA_RECOVERY);
    }
}
