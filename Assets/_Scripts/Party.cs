using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Party : MonoBehaviour {
    public PlayerQueue playerQueue;
    public GameObject playerPrefab;

    // (Name, (position, player obj)) mapping
    private Dictionary<string, Tuple<int, Player>> players = new Dictionary<string, Tuple<int, Player>>();
    [SerializeField]
    private string[] playerPos = new string[4];

    public Player CreatePlayer(PlayerCreationData data) {
        Player player = Instantiate(playerPrefab).GetComponent<Player>();
        player.stats = data.stats;
        return player;
    }

    public void EvictPlayer(string username) {
        Tuple<int, Player> player = players[username];
        playerPos[player.Item1] = null;
        players.Remove(username);
    }

    public void TryFillPartySlot(int index) {
        if(playerPos[index] != null) {
            return;
        } else {
            PlayerCreationData data = GetNPlayers(1)[0];
            if(data != null) {
                Player p = CreatePlayer(data);
                playerPos[index] = data.name;
                players.Add(data.name, new Tuple<int, Player>(index, p));
            }
        }
    }

    public Tuple<int, Player> GetPlayer(string username) {
        return players[username];
    }

    public Tuple<int, Player> GetPlayer(int index) {
        string username = playerPos[index];
        return players[username];
    }

    public List<string> GetPlayersPosition() {
        List<string> pos = new List<string>(playerPos);
        return pos;
    }

    public List<Player> GetPlayersInPosition() {
        List<Player> pos = new List<Player>();
        for(int i = 0; i < playerPos.Length; i++) {
            pos.Add(GetPlayer(i).Item2);
        }
        return pos;
    }

    public void TryFillAllPartySlots() {
        for(int i = 0; i < playerPos.Length; i++) {
            TryFillPartySlot(i);
        }
    }

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

    IEnumerator TestPlayers() {
        List<PlayerCreationData> players = GetNPlayers(2);
        for(int i = 0; i < players.Count; i++) {
            Debug.Log("Deploying Player: " + players[i].name + " - " + players[i].job.ToString());
            players[i].stats.LogStats();
            playerQueue.Remove(players[i].name);
        }
        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(TestPlayers());
    }
}
