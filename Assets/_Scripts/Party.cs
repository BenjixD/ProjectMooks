using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Party : MonoBehaviour {
    public PlayerQueue playerQueue;
    public Player heroPrefab;    // Optional TODO: Dependent on streamer
    public Player mookPrefab; // TODO: Dependent on JOB

    // (Name, (position, player obj)) mapping
    private Dictionary<string, Tuple<int, Player>> players = new Dictionary<string, Tuple<int, Player>>();
    [SerializeField]
    private PlayerCreationData[] playerPos = new PlayerCreationData[4];

    public void CreatePlayer(PlayerCreationData data, int index) {
        playerPos[index] = data;
    }

    public void CreateHeroPlayer() {
        PlayerStats stats = new PlayerStats(heroPrefab.stats);
        PlayerCreationData heroData = new PlayerCreationData(GameManager.Instance.chatBroadcaster._channelToConnectTo, stats, Job.HERO);
        CreatePlayer(heroData, 0);
    }

    public Player InstantiatePlayer(int index) {
        PlayerCreationData data = playerPos[index];
        if (data == null) {
            Debug.LogError("Error: Player not found!");
            return null;
        }

        Player player;
        if (index == 0) {
            player = Instantiate(heroPrefab).GetComponent<Player>();
        } else {
            player = Instantiate(mookPrefab).GetComponent<Player>();
        }

        player.Initialize(data);
        players.Add(data.name, new Tuple<int, Player>(index, player));

        Debug.Log("Created player: " + data.name);
        Debug.Log("Player actions: " + player.actions);

        return player;
    }


    public void TryFillPartySlot(int index) {
        if(playerPos[index] != null) {
            return;
        } else {
            List<PlayerCreationData> nPlayers = GetNPlayers(1);
            if (nPlayers.Count == 0) {
                return;
            }

            PlayerCreationData data = nPlayers[0];
            if(data != null) {
                CreatePlayer(data, index);
            }
        }
    }

    public int GetNumPlayersInParty() {
        int numPlayers = 0;
        for(int i = 0; i < playerPos.Length; i++) {
            if (playerPos[i] != null) {
                numPlayers++;
            }
        }

        return numPlayers;
    }
    
    public void EvictPlayer(string username) {
        Tuple<int, Player> player = players[username];
        playerPos[player.Item1] = null;
        players.Remove(username);
    }

    public Tuple<int, Player> GetPlayer(string username) {
        return players[username];
    }

    public Tuple<int, Player> GetPlayer(int index) {
        if (playerPos[index] == null) {
            return null;
        }

        string username = playerPos[index].name;
        if (username == "") {
            return null;
        }

        return players[username];
    }

    public List<PlayerCreationData> GetPlayersPosition() {
        List<PlayerCreationData> pos = new List<PlayerCreationData>();
        for (int i = 0; i < playerPos.Length; i++) {
            if(playerPos[i] != null) {
                pos.Add(playerPos[i]);
            }
        }
    
        return pos;
    }

    public List<Player> GetPlayersInPosition() {
        List<Player> pos = new List<Player>();
        for(int i = 0; i < playerPos.Length; i++) {
            Tuple<int, Player> player = GetPlayer(i);
            if (player != null) {
                pos.Add(player.Item2);
            }  
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
