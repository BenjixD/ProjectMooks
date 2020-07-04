

using UnityEngine;
using System.Collections.Generic;

public class ScriptableObjectDictionary : MonoBehaviour {




    public Dictionary<Job, List<ActionBase>> playerJobActions = new Dictionary<Job, List<ActionBase>>();

    public Dictionary<Job, List<ActionBase>> enemyJobActions = new Dictionary<Job, List<ActionBase>>();



    [SerializeField] private JobActionsList[] _playerJobActionsLists = null;
    [SerializeField] private JobActionsList[] _enemyJobActionsLists = null;


    [SerializeField] private List<ZoneProperties> zones = null;

    [SerializeField] private List<ActionBase> commonActions = null;

    [SerializeField] private List<ActionBase> commonMookActionPool = null;

    [SerializeField] private List<StatusAilment> commonStatusAilments = null;

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

    public List<JobActionsList> GetAllJobActionLists() {
        List<JobActionsList> actionLists = new List<JobActionsList>();
        for (int i = 0; i < _playerJobActionsLists.Length; i++) {
            if (_playerJobActionsLists != null) {
                actionLists.Add(_playerJobActionsLists[i]);
            }
        }

        for (int i = 0; i < _enemyJobActionsLists.Length; i++) {
            if (_enemyJobActionsLists != null) {
                actionLists.Add(_enemyJobActionsLists[i]);
            }
        }

        return actionLists;
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

    public List<ZoneProperties> GetZoneProperties() {
        return zones;
    }

    public ActionBase GetCommonAction(string name) {
        return this.commonActions.Find(action => action.name == name);
    }

    public StatusAilment GetCommonStatusAilment(string name) {
        return this.commonStatusAilments.Find(ailment => ailment.name == name);
    }

    public StatBuffAilment GetStatBuffAilment(StatType stat, float value) {
        StatBuffAilment ailment = (StatBuffAilment)this.commonStatusAilments.Find(ail => ail.name == "StatBuff");
        ailment = Instantiate(ailment);
        ailment.statType = stat;
        ailment.val = value;

        return ailment;
    }

    public List<ActionBase> GetCommonMookActionPool() {
        return this.commonMookActionPool;
    }
}
