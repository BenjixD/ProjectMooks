using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PartyCreationData : MonoBehaviour {
    public const int numPlayers = 4;
    public PlayerQueue playerQueue;
    private PlayerCreationData[] playerPos = new PlayerCreationData[numPlayers];

    public PlayerCreationData[] GetPlayerCreationData() {
        return playerPos;
    }

    public void Initialize(string heroName) {
        // Creates the hero player
        JobActionsList jobActionsList = GameManager.Instance.GetPlayerJobActionsList(Job.HERO);
        FightingEntity heroPrefab = jobActionsList.prefab;
        PlayerStats stats = new PlayerStats(heroPrefab.stats);
        PlayerCreationData heroData = new PlayerCreationData(heroName, stats, Job.HERO);
        CreatePlayer(heroData, 0);
    }

    public void CreatePlayer(PlayerCreationData data, int index) {
        playerPos[index] = data;
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
    
    public void EvictPlayer(int index) {
        PlayerCreationData playerData = playerPos[index];
        playerPos[index] = null;
        playerQueue.Remove(playerData.name);
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

    public void TryFillAllPartySlots() {
        for(int i = 0; i < playerPos.Length; i++) {
            TryFillPartySlot(i);
        }
    }

    private List<PlayerCreationData> GetNPlayers(int n) {
        List<PlayerCreationData> players = new List<PlayerCreationData>();
        for(int i = 0; i < n; i++) {
            PlayerCreationData data = playerQueue.Dequeue();
            if(data != null) {
                players.Add(data);
            }
        }
        return players;
    }
}
