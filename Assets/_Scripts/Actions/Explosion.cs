using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Explosion", menuName = "Actions/Quick Time Events/Explosion", order = 3)]
public class Explosion : ActionBase {
    [SerializeField] private GameObject _explosionQTE = null;

    public override bool QueueAction(FightingEntity user, string[] splitCommand) {
        List<int> targetIds = this.GetAllPossibleTargetIds();
        user.SetQueuedAction(this, targetIds);
        return true;
    }

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        ExplosionQTE qte = Instantiate(_explosionQTE).GetComponent<ExplosionQTE>();
        qte.Initialize(user, targets, this);
        return new FightResult(user, this);
    }
    
    public void FinishQTE(FightingEntity user, List<FightingEntity> targets, float power) {
        PlayerStats before, after;
        List<DamageReceiver> receivers = new List<DamageReceiver>();
        int rawDamage = GetRawDamage(user.stats, power);
        
        foreach (FightingEntity target in targets) {
            int damage = (int) (rawDamage * target.stats.GetDamageMultiplierResistance());
            if (!target.IsHeroTeam()) {
                damage *= 2;
            }
            InstantiateDamagePopup(target, damage);

            before = (PlayerStats)target.stats.Clone();
            target.stats.hp.ApplyDelta(-damage);
            after = (PlayerStats)target.stats.Clone();

            receivers.Add(new DamageReceiver(target, before, after));
        }
        
        FightResult result = new FightResult(user, this, receivers);
        _battleFight.EndFight(result, this);
    }

    // Return damage based on explosion power and user's stats (without accounting for the targets' resistances)
    public int GetRawDamage(PlayerStats stats, float power) {     
        int damage = (int) (stats.physical.GetValue() * effects.physicalScaling + stats.special.GetValue() * effects.specialScaling);
        return (int) (power * damage);
    }
}
