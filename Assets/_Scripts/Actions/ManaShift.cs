using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ManaShift", menuName = "Actions/Quick Time Events/Mana Shift", order = 3)]
public class ManaShift : ActionBase {
    [SerializeField] private GameObject _manaShiftQTE = null;

    protected override bool QueueAction(FightingEntity user, string[] splitCommand) {
        user.SetQueuedAction(new QueuedAction(user, this, new List<int>{ GameManager.Instance.battleComponents.field.GetHeroPlayer().targetId } ));
        return true;
    }

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        Instantiate(_manaShiftQTE);

        // TODO: integration with battle controller
        FinishQTE(user, targets, 0);
        
        return new FightResult(user, this);
    }

    public FightResult FinishQTE(FightingEntity user, List<FightingEntity> targets, float power) {
        int manaRestored = (int) (user.stats.special.GetValue() * power);
        PlayerStats heroStats = targets[0].stats;
        heroStats.mana.ApplyDelta(manaRestored);
        return new FightResult(user, this);
    }
}
