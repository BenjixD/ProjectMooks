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

        party.CreateHeroPlayer();
    }

    public List<ActionBase> GetPlayerJobActionsList(Job job) {
        if(!playerJobActions.ContainsKey(job)) {
            Debug.Log("No JobActionsList for job " + job + " found");
            return null;
        }
        return playerJobActions[job];
    }

    public List<ActionBase> GetEnemyJobActionsList(Job job) {
        if(!enemyJobActions.ContainsKey(job)) {
            Debug.Log("No JobActionsList for job " + job + " found");
            return null;
        }
        return enemyJobActions[job];
    }

    public FightingEntity GetPrefabForPlayerJob(Job job) {
        foreach(JobActionsList jobActionsList in _playerJobActionsLists) {
            if (jobActionsList.job == job) {
                return jobActionsList.prefab;
            }
        }

        return null;
    }

    public FightingEntity GetPrefabForEnemyJob(Job job) {
        foreach(JobActionsList jobActionsList in _enemyJobActionsLists) {
            if (jobActionsList.job == job) {
                return jobActionsList.prefab;
            }
        }

        return null;
    }

}