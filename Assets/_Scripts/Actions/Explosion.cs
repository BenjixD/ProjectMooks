using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Explosion", menuName = "Actions/Explosion", order = 2)]
public class Explosion : ActionBase {
    [SerializeField] private GameObject _explosionQTE;

    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        if (!base.TryChooseAction(user, splitCommand)) {
            return false;
        }

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
            int defence = target.stats.GetResistance();
            int damage = Mathf.Max(attackDamage - defence, 0);

            before = (PlayerStats)target.stats.Clone();
            target.stats.SetHp(target.stats.GetHp() - damage);
            after = (PlayerStats)target.stats.Clone();

            receivers.Add(new DamageReceiver(target, before, after));
        }

        return new FightResult(user, this);
    }

    // Return damage based on explosion power and user's stats (without accounting for the targets' resistances)
    public int GetRawDamage(PlayerStats stats, float power) {     
        // TODO: balance damage formula
        return (int) (power * stats.GetSpecial());
    }
}
