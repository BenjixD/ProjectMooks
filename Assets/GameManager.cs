using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    public Party party;
    public BattleController battleController{get; set;}

    public TwitchChatBroadcaster chatBroadcaster;

    public List<Enemy> enemies{get; set;}

    [SerializeField]
    List<Enemy> _enemyPrefabs;

    [SerializeField] private JobActionsList[] _jobActionsLists;
    public Dictionary<Job, List<ActionBase>> jobActions = new Dictionary<Job, List<ActionBase>>();
    

    void Awake() {
        foreach(JobActionsList jobActionsList in _jobActionsLists) {
            jobActions.Add(jobActionsList.job, jobActionsList.GetActions());
        }

        PlayerStats stats = new PlayerStats();
        PlayerCreationData heroData = new PlayerCreationData(chatBroadcaster._channelToConnectTo, stats, Job.HERO);
        party.CreatePlayer(heroData, 0);
 


    }

    public void GenerateEnemyList() {
        int numberOfEnemiesToGenerate = 1; // TODO: Make this dependent on stage.
        enemies = new List<Enemy>();
        List<Enemy> validEnemies = new List<Enemy>(_enemyPrefabs);  // TODO: make validEnemies dependent on the level - best done in a JSON object


        for (int i = 0; i < numberOfEnemiesToGenerate; i++) {
            int enemyIndex = Random.Range(0, validEnemies.Count);

            Enemy enemyPrefab = validEnemies[enemyIndex];
            Enemy instantiatedEnemy = Instantiate(enemyPrefab) as Enemy;

            PlayerStats stats = new PlayerStats();
            PlayerCreationData creationData = new PlayerCreationData("Evil monster", stats, Job.BASIC_ENEMY);
            instantiatedEnemy.Initialize(creationData);

            enemies.Add(instantiatedEnemy);
        }

        if (enemies.Count == 0) {
            Debug.LogError("ERROR: No enemies found!");
        }

    }

    public List<FightingEntity> getAllFightingEntities() {
        List<Player> players = GameManager.Instance.party.GetPlayersInPosition();
        List<FightingEntity> entities = new List<FightingEntity>();
        foreach (var player in players) {
            entities.Add(player);
        }

        foreach (var enemy in enemies) {
            entities.Add(enemy);
        }

        return entities;
    }

    public List<ActionBase> GetJobActionsList(Job job) {
        if(!jobActions.ContainsKey(job)) {
            Debug.Log("No JobActionsList for job " + job + " found");
            return null;
        }
        return jobActions[job];
    }
}