
using UnityEngine;
using System.Collections.Generic;

// This class stores the current game state, not specific to any particular scene.
// Might not need to be MonoBehaviour, but I'll leave it open for now
[RequireComponent (typeof(ProgressData))]
public class GameState : MonoBehaviour {

    public PlayerParty playerParty;
    public EnemyParty enemyParty;


    public ProgressData progressData;

    // TODOL
    // public List<StageInfoContainer> stages {get; set;}

    private StageInfoContainer _currStage;

    // TODOL
    public Queue<WaveInfo> waves;

    public void Initialize() {
        // TODOL
        // this.InitializeStages();
    }

  
    public StageInfoContainer GetCurrentStage() {
        // TODOL
        return _currStage;
        // return this.stages[this.progressData.currentStageIndex];
    }

    // TODOL
    // public void SetStageIndex(int index) {
    //     this.progressData.currentStageIndex = index;
    // }

    // public void SetNextStageIndex(int index) {
    //     this.progressData.nextStageIndex = index;
    // }

    // TODOL
    // private void InitializeStages() {
    //     this.stages = new List<StageInfoContainer>();
    //     List<StageInfo> stages = GameManager.Instance.models.GetStageInfos();
    //     foreach (StageInfo stage in stages) {
    //         StageInfoContainer stageInfo = new StageInfoContainer(stage);
    //         stageInfo.InitializeWaveList(); // Does random generation
    //         this.stages.Add(stageInfo);
    //     }
    // }

    public void InitializeStage(StageInfoContainer stageInfoContainer) {
        // TODOL
        _currStage = stageInfoContainer;
    }
}