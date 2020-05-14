

using UnityEngine;
using System.Collections.Generic;

public class ScriptableObjectDictionary : MonoBehaviour {

    public Dictionary<Job, List<ActionBase>> playerJobActions = new Dictionary<Job, List<ActionBase>>();

    public Dictionary<Job, List<ActionBase>> enemyJobActions = new Dictionary<Job, List<ActionBase>>();

    [SerializeField] private JobActionsList[] _playerJobActionsLists;
    [SerializeField] private JobActionsList[] _enemyJobActionsLists;


    [SerializeField]
    private List<StageInfo> stageInfo;

    public void Initialize() {
        foreach(JobActionsList jobActionsList in _playerJobActionsLists) {
            playerJobActions.Add(jobActionsList.job, jobActionsList.GetActions());
        }

        foreach(JobActionsList jobActionsList in _enemyJobActionsLists) {
            enemyJobActions.Add(jobActionsList.job, jobActionsList.GetActions());
        }
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

    public List<StageInfo> GetStageInfos() {
        return this.stageInfo;
    }
}
