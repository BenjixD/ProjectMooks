using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ManaShift", menuName = "Actions/Mana Shift", order = 2)]
public class ManaShift : ActionBase {
    [SerializeField] private GameObject _manaShiftQTE;

    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        // Command format: !shift
        if (!BasicValidation(splitCommand)) {
            return false;
        }

        user.SetQueuedAction(new QueuedAction(user, this, new List<int>{ GameManager.Instance.turnController.field.GetHeroPlayer().targetId } ));
        return true;
    }

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        Instantiate(_manaShiftQTE);

        // TODO: integration with battle controller
        FinishQTE(user, targets, 0);
        
        return new FightResult(user, this);
    }

    public FightResult FinishQTE(FightingEntity user, List<FightingEntity> targets, float power) {
        int manaRestored = (int) (user.stats.GetSpecial() * power);
        Player hero = (Player) targets[0];
        hero.stats.SetMana(hero.stats.GetMana() + manaRestored);
        return new FightResult(user, this);
    }
}
