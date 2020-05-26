using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// A Singleton bridge to access data from across the application
public class GameManager : Singleton<GameManager> {
    public TwitchChatBroadcaster chatBroadcaster;

    public ScriptableObjectDictionary models;

    public GameState gameState;
    public TimeManager time;

    // Note: This field is null if not in battle
    public TurnController turnController{get; set;}

    void Awake() {
        if (FindObjectsOfType<GameManager>().Length >= 2) {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        this.models.Initialize();
        gameState.playerParty.Initialize();
        gameState.enemyParty.Initialize();


        // Initialize with the field for debugging purposes
        if (SceneManager.GetActiveScene().name != "_MainMenu") {
            chatBroadcaster.ConnectToChannel(chatBroadcaster._channelToConnectTo);
            gameState.playerParty.CreateHero(chatBroadcaster._channelToConnectTo);
        }

        this.gameState.Initialize();
    }

}