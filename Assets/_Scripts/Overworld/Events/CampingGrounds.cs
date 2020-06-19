using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampingGrounds : EventStage {
    protected override void BeginEvent() {
        PlayerStats heroStats = GameManager.Instance.gameState.playerParty.GetFighters<Player>()[0].playerCreationData.stats;
        heroStats.hp.ClearModifiers();
        heroStats.mana.ClearModifiers();
    }
}
