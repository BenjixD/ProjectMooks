using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampingGrounds : EventStage {
    protected override void BeginEvent() {
        PlayerStats heroStats = GameManager.Instance.gameState.playerParty.GetFighters<Player>()[0].stats;
        heroStats.hp.SetValue(heroStats.maxHp.GetValue());
        heroStats.mana.SetValue(heroStats.maxMana.GetValue());
    }
}
