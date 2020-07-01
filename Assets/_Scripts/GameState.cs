
using UnityEngine;
using System.Collections.Generic;

// This class stores the current game state, not specific to any particular scene.
// Might not need to be MonoBehaviour, but I'll leave it open for now
[RequireComponent (typeof(ProgressData))]
public class GameState : MonoBehaviour {

    public PlayerParty playerParty;
    public EnemyParty enemyParty;


    public ProgressData progressData;

    private StageInfoContainer _currStage;

    public StageInfoContainer GetCurrentStage() {
        return _currStage;
    }

    public void SetStage(StageInfoContainer stageInfoContainer) {
        _currStage = stageInfoContainer;
    }
}