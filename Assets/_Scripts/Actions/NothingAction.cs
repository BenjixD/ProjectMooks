using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nothing", menuName = "Actions/Nothing", order = 2)]
public class NothingAction : ActionBase {
    public override ActionChoiceResult TryChooseAction(FightingEntity user, string[] splitCommand) {
        return new ActionChoiceResult(ActionChoiceResult.State.CANNOT_BE_QUEUED, new List<string>{"Not a valid move"});
    }

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
		return new FightResult(user, this);	
    }
}
