using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : FightingEntity {
    public bool isHero = false;

    void Start() {
    }

	public void Initialize(PlayerCreationData data) {
		Name = data.name;
		SetStats(data.stats);
		SetJob(data.job);
	}

	// Getters / Setters 
	public string GetJobName() {
		return job.ToString();
	}

    public void OnCommandReceived(string username, string message) {
        if (username == Name) {
            this.HandleCommand(message);
        }
    }

    public void HandleCommand(string message) {
        string[] split = message.Split(' ');
        if (split.Length < 1) {
            Debug.Log("Invalid battle command");
            return;
        }

        // Right now, format is: action target
        // action is one of: Attack(A), Magic(M), Defend(D)
        // target is an integer 1, 2, 3...

        string action = split[0];

        int target = 1;

        int.TryParse(split[1], out target);

        switch (action) {
            case "a":
            case "attack":
                command.option = BattleOption.ATTACK;
            break;

            case "magic":
            case "m":
                command.option = BattleOption.MAGIC;
            break;

            case "defend":
            case "d":
                command.option = BattleOption.DEFEND;
            break;

            default:
                Debug.Log("Invalid battle command");
            break;
        }

        if (target <= 0 || target >= GameManager.Instance.enemies.Count) {
            Debug.Log("Invalid target");
            return;
        }

         command.target = target - 1;

         this.hasSetCommand = true;
    }
}

