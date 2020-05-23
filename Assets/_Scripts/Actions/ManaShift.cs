using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ManaShift", menuName = "Actions/Mana Shift", order = 2)]
public class ManaShift : ActionBase {
    [SerializeField] private GameObject _manaShiftQTE;

    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        // Defend command format: !shift
        if (!BasicValidation(splitCommand)) {
            return false;
        }

        user.SetQueuedAction(new QueuedAction(user, this, new List<int>{ GameManager.Instance.turnController.field.GetHeroPlayer().targetId } ));
        return true;
    }

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        ManaShiftQTE qte = Instantiate(_manaShiftQTE).GetComponent<ManaShiftQTE>();
        // qte.SetCallback(FinishQTE(user.stats.GetSpecial()));
        // TODO: process QTE + execute effect
        // TODO: pause battle controller

        
        int manaRestored = user.stats.GetSpecial();
        Player hero = (Player) targets[0];
        hero.stats.SetMana(hero.stats.GetMana() + manaRestored);

        return new FightResult(user, this);
    }
}
