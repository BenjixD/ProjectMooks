using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ThunderOne", menuName = "Actions/Thunder One", order = 2)]
public class ThunderOneTest : ActionBase {
    [SerializeField]
    private List<StatusAilment> _status;

    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        // Fire One command format: !m fire [target number]
        if (!BasicValidation(splitCommand)) {
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

            foreach(StatusAilment sa in _status) {
                target.GetAilmentController().AddStatusAilment(Instantiate(sa));
            }

            receivers.Add(new DamageReceiver(target, before, after, _status));
        }

        return new FightResult(user, this, receivers);
    }
}