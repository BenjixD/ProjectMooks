using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Paralyzed", menuName = "Actions/Ailment Actions/Paralyzed", order = 4)]
public class Paralyzed : ActionBase {
    public override ActionChoiceResult TryChooseAction(FightingEntity user, string[] splitCommand) {
        return new ActionChoiceResult(ActionChoiceResult.State.CANNOT_BE_QUEUED, new List<string>{"Not a valid move"});
    }

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
    	Debug.Log("PARALYZED");
		return new FightResult(user, this);	
    }
}
