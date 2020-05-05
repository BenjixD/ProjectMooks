using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    [SerializeField] private JobActionsList[] _jobActionsLists;
    public Dictionary<Job, Dictionary<ActionType, ActionBase>> jobActions = new Dictionary<Job, Dictionary<ActionType, ActionBase>>();
    
    private void Start() {
        foreach(JobActionsList jobActionsList in _jobActionsLists) {
            jobActionsList.Initialize();
            jobActions.Add(jobActionsList.job, jobActionsList.GetActions());
        }
    }

    public Dictionary<ActionType, ActionBase> GetJobActionsList(Job job) {
        if(!jobActions.ContainsKey(job)) {
            Debug.Log("No JobActionsList for job " + job + " found");
            return null;
        }
        return jobActions[job];
    }
}