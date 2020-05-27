using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoisonAllTest", menuName = "Actions/Poison All", order = 2)]
public class PoisonAllTest : ActionBase {
    [SerializeField]
    private List<StatusAilment> _status;

    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        // Poison All command format: !m poison2
        if (!BasicValidation(splitCommand)) {
            return false;
        }
        List<int> targetIds = this.GetAllPossibleTargets(user).Map((FightingEntity target) => target.targetId );
        user.SetQueuedAction(new QueuedAction(user, this, targetIds));
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

            foreach(StatusAilment sa in _status) {
                target.GetAilmentController().AddStatusAilment(Instantiate(sa));
            }

            receivers.Add(new DamageReceiver(target, before, after));
        }

        return new FightResult(user, this, receivers);
    }
}
