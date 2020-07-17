using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ManaShift", menuName = "Actions/Quick Time Events/Mana Shift", order = 3)]
public class ManaShift : ActionBase {
    [SerializeField] private GameObject _manaShiftQTE = null;

    public override bool QueueAction(FightingEntity user, string[] splitCommand) {
        user.SetQueuedAction(this, new List<int>{ GameManager.Instance.battleComponents.field.GetHeroPlayer().targetId });
        return true;
    }

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        ManaShiftQTE qte = Instantiate(_manaShiftQTE).GetComponent<ManaShiftQTE>();
        qte.Initialize(user, targets, this);
        return new FightResult(user, this);
    }

    public FightResult FinishQTE(FightingEntity user, List<FightingEntity> targets, float power) {
        int manaRestored = GetManaRestored(user, power);
        InstantiateDamagePopup(targets[0], manaRestored, DamageType.MANA_RECOVERY);
        PlayerStats heroStats = targets[0].stats;
        heroStats.mana.ApplyDelta(manaRestored);
        return new FightResult(user, this);
    }

    public int GetManaRestored(FightingEntity user, float power) {
        float baseMana = user.stats.physical.GetValue() * effects.physicalScaling + user.stats.special.GetValue() * effects.specialScaling;
        int manaRestored = (int) (baseMana * power);
        return manaRestored;
    }
}
