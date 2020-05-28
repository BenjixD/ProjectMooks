using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nothing", menuName = "Actions/Nothing", order = 2)]
public class NothingAction : ActionBase {
    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        return false;
    }

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
        return new FightResult(user, this);
    }
}
