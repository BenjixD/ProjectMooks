using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Paralyzed", menuName = "Actions/Paralyzed", order = 2)]
public class Paralyzed : ActionBase {
    public override bool TryChooseAction(FightingEntity user, string[] splitCommand) {
        return false;
    }

    public override FightResult ApplyEffect(FightingEntity user, List<FightingEntity> targets) {
    	Debug.Log("PARALYZED");
		return new FightResult(user, this);	
    }
}
