using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EnemyParty : Party {
    public const int maxPlayers = 4;

    // Initialize fighters array
    public override void Initialize() {
        this.fighters = new Enemy[EnemyParty.maxPlayers];
    }
}
