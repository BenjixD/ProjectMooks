using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    public Party party;
    public TurnController turnController{get; set;}

    public TwitchChatBroadcaster chatBroadcaster;

    [SerializeField] private JobActionsList[] _playerJobActionsLists;
    [SerializeField] private JobActionsList[] _enemyJobActionsLists;

    public Dictionary<Job, List<ActionBase>> playerJobActions = new Dictionary<Job, List<ActionBase>>();

    public Dictionary<Job, List<ActionBase>> enemyJobActions = new Dictionary<Job, List<ActionBase>>();
    
    public List<StageInfoContainer> stages {get; set;}

    public int currentStageIndex = 0;


    [SerializeField]
    private List<StageInfo> stageInfo = new List<StageInfo>();

    void Awake() {
        if (FindObjectsOfType<GameManager>().Length >= 2) {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        foreach(JobActionsList jobActionsList in _playerJobActionsLists) {
            playerJobActions.Add(jobActionsList.job, jobActionsList.GetActions());
        }

        foreach(JobActionsList jobActionsList in _enemyJobActionsLists) {
            enemyJobActions.Add(jobActionsList.job, jobActionsList.GetActions());
        }

        this.InitializeStages();

        party.CreateHeroPlayer();

    }

    public List<ActionBase> GetPlayerJobActions(Job job) {
        if(!playerJobActions.ContainsKey(job)) {
            Debug.Log("No JobActionsList for job " + job + " found");
            return null;
        }
        return playerJobActions[job];
    }

    public List<ActionBase> GetEnemyJobActions(Job job) {
        if(!enemyJobActions.ContainsKey(job)) {
            Debug.Log("No JobActionsList for job " + job + " found");
            return null;
        }
        return enemyJobActions[job];
    }

    public JobActionsList GetPlayerJobActionsList(Job job) {
        foreach(JobActionsList jobActionsList in _playerJobActionsLists) {
            if (jobActionsList.job == job) {
                return jobActionsList;
            }
        }

        return null;
    }

    public JobActionsList GetEnemyJobActionsList(Job job) {
        foreach(JobActionsList jobActionsList in _enemyJobActionsLists) {
            if (jobActionsList.job == job) {
                return jobActionsList;
            }
        }

        return null;
    }

    public List<Job> getMookJobs() {
        List<Job> jobs = new List<Job>(playerJobActions.Keys);
        jobs.Remove(Job.HERO);
        return jobs;
    }

    public StageInfoContainer GetCurrentStage() {
        return this.stages[this.currentStageIndex];
    }

    public void SetStageIndex(int index) {
        this.currentStageIndex = index;
    }


    private void InitializeStages() {
        this.stages = new List<StageInfoContainer>();
        foreach (StageInfo stage in stageInfo) {
            StageInfoContainer stageInfo = new StageInfoContainer(stage);
            stageInfo.InitializeWaveList(); // Does random generation
            this.stages.Add(stageInfo);
        }
    }


}