using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Explosion", menuName = "Actions/Explosion", order = 2)]
public class Explosion : ActionBase {
    [SerializeField] private GameObject _explosionQTE;

    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        // Command format: !explosion
        if (!BasicValidation(splitCommand)) {
            return false;
        }

        List<int> targetIds = this.GetAllPossibleTargets(user).Map((FightingEntity target) => target.targetId );
        user.SetQueuedAction(new QueuedAction(user, this, targetIds));
        return true;
    }

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        Instantiate(_explosionQTE);
        // TODO: process QTE + execute explosion effect

        List<DamageReceiver> receivers = new List<DamageReceiver>();
        int attackDamage = user.stats.GetSpecial();
        
        foreach (FightingEntity target in targets) {
            PlayerStats before, after;
            int defence = target.stats.GetResistance();
            int damage = Mathf.Max(attackDamage - defence, 0);

            before = (PlayerStats)target.stats.Clone();
            target.stats.SetHp(target.stats.GetHp() - damage);
            after = (PlayerStats)target.stats.Clone();

            receivers.Add(new DamageReceiver(target, before, after));
        }

        return new FightResult(user, this, receivers);
    }
}
