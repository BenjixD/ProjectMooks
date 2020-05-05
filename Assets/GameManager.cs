using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    [SerializeField] private JobActionsList[] _jobActionsLists;
    public Dictionary<Job, List<ActionBase>> jobActions = new Dictionary<Job, List<ActionBase>>();
    
    private void Awake() {
        foreach(JobActionsList jobActionsList in _jobActionsLists) {
            jobActions.Add(jobActionsList.job, jobActionsList.GetActions());
        }
    }

    public List<ActionBase> GetJobActionsList(Job job) {
        if(!jobActions.ContainsKey(job)) {
            Debug.Log("No JobActionsList for job " + job + " found");
            return null;
        }
        return jobActions[job];
    }
}