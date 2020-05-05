using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
	public PlayerQueue playerQueue;

    public BattleController battleController{get; set;}

    public TwitchChatBroadcaster chatBroadcaster;

    public List<Player> players = new List<Player>();

    public List<Enemy> enemies = new List<Enemy>();

    [SerializeField]
    List<Enemy> _enemyPrefabs;


    public int level = 1;


	List<PlayerCreationData> GetNPlayers(int n) {
		List<PlayerCreationData> players = new List<PlayerCreationData>();
		for(int i = 0; i < n; i++) {
			PlayerCreationData data = playerQueue.Dequeue();
			if(data != null) {
				players.Add(data);
			}
		}
		return players;
	}

    void Awake() {
        //heroPlayer = new Player(chatBroadcaster._channelToConnectTo);
    }

	void Start() {
		StartCoroutine(TestPlayers());
	}

	void Update() {

	}

    public void GenerateEnemyList() {
        int numberOfEnemiesToGenerate = 1; // TODO: Make this dependent on stage.
        enemies.Clear();
        List<Enemy> validEnemies = new List<Enemy>(_enemyPrefabs);  // TODO: make validEnemies dependent on the level - best done in a JSON object


        for (int i = 0; i < numberOfEnemiesToGenerate; i++) {
            int enemyIndex = Random.Range(0, validEnemies.Count);

            Enemy enemyPrefab = validEnemies[enemyIndex];
            Enemy instantiatedEnemy = Instantiate(enemyPrefab) as Enemy;

            enemies.Add(instantiatedEnemy);
        }

        if (enemies.Count == 0) {
            Debug.LogError("ERROR: No enemies found!");
        }

    }

    public List<FightingEntity> getAllFightingEntities() {
        List<FightingEntity> entities = new List<FightingEntity>();
        foreach (var player in players) {
            entities.Add(player);
        }

        foreach (var enemy in enemies) {
            entities.Add(enemy);
        }

        return entities;
    }

	IEnumerator TestPlayers() {
		List<PlayerCreationData> players = GetNPlayers(2);
		for(int i = 0; i < players.Count; i++) {
			Debug.Log("Deploying Player: " + players[i].name);
			playerQueue.Remove(players[i].name);
		}
		yield return new WaitForSeconds(5f);
		yield return StartCoroutine(TestPlayers());
	}




}