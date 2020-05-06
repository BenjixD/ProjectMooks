using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    public Party party;
    public BattleController battleController{get; set;}

    public TwitchChatBroadcaster chatBroadcaster;

  


    [SerializeField] private JobActionsList[] _jobActionsLists;
    public Dictionary<Job, List<ActionBase>> jobActions = new Dictionary<Job, List<ActionBase>>();
    

    void Awake() {
        if (FindObjectsOfType<GameManager>().Length >= 2) {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        foreach(JobActionsList jobActionsList in _jobActionsLists) {
            jobActions.Add(jobActionsList.job, jobActionsList.GetActions());
        }


        if (party.GetPlayersInPosition().Count == 0) {
            PlayerStats stats = new PlayerStats();
            // TODO: change this later
            stats.maxHp = 100;
            stats.maxMana = 100;
            stats.maxPhysical = 20;
            stats.maxSpecial = 20;
            stats.maxSpeed = 35;
            stats.maxDefense = 10;
            stats.maxResistance = 10;
            stats.RandomizeStats();
            
            PlayerCreationData heroData = new PlayerCreationData(GameManager.Instance.chatBroadcaster._channelToConnectTo, stats, Job.HERO);
            GameManager.Instance.party.CreatePlayer(heroData, 0);
        }
    }

    public List<ActionBase> GetJobActionsList(Job job) {
        if(!jobActions.ContainsKey(job)) {
            Debug.Log("No JobActionsList for job " + job + " found");
            return null;
        }
        return jobActions[job];
    }

    public FightingEntity GetPrefabForJob(Job job) {
        foreach(JobActionsList jobActionsList in _jobActionsLists) {
            if (jobActionsList.job == job) {
                return jobActionsList.prefab;
            }
        }

        return null;
    }
}