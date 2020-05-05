using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
    public Party party;
    public BattleController battleController{get; set;}

    public TwitchChatBroadcaster chatBroadcaster;

    public List<Enemy> enemies = new List<Enemy>();

    [SerializeField]
    List<Enemy> _enemyPrefabs;

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

}