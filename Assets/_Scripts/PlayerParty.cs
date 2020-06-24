using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerParty : Party {
    public const int maxPlayers = 4;


    public PlayerQueue playerQueue;

    public override void Initialize() {
        this.fighters = new Player[PlayerParty.maxPlayers];
    }

    public void CreateHero(string heroName) {
        // Creates the hero player
        JobActionsList jobActionsList = GameManager.Instance.models.GetPlayerJobActionsList(Job.HERO);
        FightingEntity heroPrefab = jobActionsList.prefab;
        PlayerStats stats = (PlayerStats)heroPrefab.stats.Clone();
        // stats.RandomizeStats(); // May or may not want this...
        PlayerCreationData heroData = new PlayerCreationData(heroName, stats, Job.HERO);
        Player hero = new Player();
        hero.SetPlayerCreationData(heroData);
        hero.Initialize(jobActionsList, heroName);
        this.SetFighter(0, hero);
    }

 
    public void TryFillPartySlot(int index) {
        if(this.fighters[index] != null) {
            return;
        } else {
            List<Player> nPlayers = GetNPlayers(1);
            if (nPlayers.Count == 0) {
                return;
            }

            Player data = nPlayers[0];
            if(data != null) {
                this.SetFighter(index, data);
            }
        }
    }
    
    public void EvictPlayer(int index) {
        Player player = this.GetFighters<Player>()[index];
        playerQueue.Remove(player.playerCreationData.name);
        this.SetFighter(index, null);
    }

    public List<Player> GetPlayersPosition() {
        List<Player> pos = new List<Player>();
        Player[] players = this.GetFighters<Player>();
        for (int i = 0; i < players.Length; i++) {
            if(players[i] != null) {
                pos.Add(players[i]);
            }
        }
    
        return pos;
    }

    public void TryFillAllPartySlots() {
        for(int i = 0; i < this.fighters.Length; i++) {
            TryFillPartySlot(i);
        }
    }


    private List<Player> GetNPlayers(int n) {
        List<Player> players = new List<Player>();
        for(int i = 0; i < n; i++) {
            PlayerCreationData data = playerQueue.Dequeue();
            if(data != null) {
                Player player = new Player();
                player.Initialize(data, data.name);
                player.SetPlayerCreationData(data);
                players.Add(player);
            }
        }
        return players;
    }
}
