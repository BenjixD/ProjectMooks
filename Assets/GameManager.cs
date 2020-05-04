using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {
	public PlayerQueue playerQueue;

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

	void Start() {
		StartCoroutine(TestPlayers());
	}

	void Update() {

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