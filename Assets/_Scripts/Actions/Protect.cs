using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Protect", menuName = "Actions/Protect", order = 5)]
public class Protect : ActionBase {
    [Tooltip("The defense debuff that will be applied to the user.")]
    public AilmentInfliction debuffAilment;
    [Tooltip("The defense buff that will be applied to allies.")]
    public AilmentInfliction buffAilment;

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        PlayerStats before, after;
        List<DamageReceiver> receivers = new List<DamageReceiver>();
        foreach (FightingEntity target in targets) {
            before = (PlayerStats)target.stats.Clone();
            // Debuff user, buff others
            if (target == user) {
                target.ailmentController.TryInflictAilment(debuffAilment);
            } else {
                target.ailmentController.TryInflictAilment(buffAilment);
            }
            after = (PlayerStats)target.stats.Clone();
            receivers.Add(new DamageReceiver(user, before, after, new List<StatusAilment>() { debuffAilment.statusAilment } ));
        }
        return new FightResult(user, this, receivers);
    }
}
