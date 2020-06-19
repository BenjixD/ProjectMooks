using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Explosion", menuName = "Actions/Quick Time Events/Explosion", order = 3)]
public class Explosion : ActionBase {
    [SerializeField] private GameObject _explosionQTE;

    protected override bool QueueAction(FightingEntity user, string[] splitCommand) {
        List<int> targetIds = this.GetAllPossibleTargets(user).Map((FightingEntity target) => target.targetId );
        user.SetQueuedAction(new QueuedAction(user, this, targetIds));
        return true;
    }

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        ExplosionQTE qte = Instantiate(_explosionQTE).GetComponent<ExplosionQTE>();
        qte.Initialize(user.stats, this);

        // TODO: integration with battle controller
        FinishQTE(user, targets, 0);

        return new FightResult(user, this);
    }
    
    public FightResult FinishQTE(FightingEntity user, List<FightingEntity> targets, float power) {
        List<DamageReceiver> receivers = new List<DamageReceiver>();
        int attackDamage = GetRawDamage(user.stats, power);
        
        foreach (FightingEntity target in targets) {
            PlayerStats before, after;
            int defence = target.stats.resistance.GetValue();
            int damage = Mathf.Max(attackDamage - defence, 0);

            before = (PlayerStats)target.stats.Clone();
            target.stats.hp.ApplyDelta(-damage);
            after = (PlayerStats)target.stats.Clone();

            receivers.Add(new DamageReceiver(target, before, after));
        }

        return new FightResult(user, this);
    }

    // Return damage based on explosion power and user's stats (without accounting for the targets' resistances)
    public int GetRawDamage(PlayerStats stats, float power) {     
        // TODO: balance damage formula
        return (int) (power * stats.special.GetValue());
    }
}
