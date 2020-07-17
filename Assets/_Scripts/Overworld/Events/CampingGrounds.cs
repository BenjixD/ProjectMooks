using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampingGrounds : EventStage {
    protected override void BeginEvent() {
        List<Player> partyFighters = GameManager.Instance.gameState.playerParty.GetActiveFighters<Player>();
        foreach (Player fighter in partyFighters) {
            fighter.stats.hp.SetValue(fighter.stats.maxHp.GetValue());
            fighter.stats.mana.SetValue(fighter.stats.maxMana.GetValue());
            foreach (ActionBase action in fighter.actions) {
                action.RestorePP();
            }
        }
    }
}
