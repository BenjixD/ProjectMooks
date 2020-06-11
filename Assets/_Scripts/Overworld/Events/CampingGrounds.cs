using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampingGrounds : EventRoom {
    protected override void BeginEvent() {
        PlayerStats heroStats = GameManager.Instance.gameState.playerParty.GetFighters<Player>()[0].playerCreationData.stats;
        heroStats.SetHp(heroStats.maxHp);
        heroStats.SetMana(heroStats.maxMana);
    }
}
