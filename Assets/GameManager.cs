using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// A Singleton bridge to access data from across the application
public class GameManager : Singleton<GameManager> {
    public PartyCreationData party;
    public TwitchChatBroadcaster chatBroadcaster;

    public ScriptableObjectDictionary models;

    public GameState gameState;

    // Note: This field is null if not in battle
    public TurnController turnController{get; set;}

    void Awake() {
        if (FindObjectsOfType<GameManager>().Length >= 2) {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        this.models.Initialize();

        // Initialize with the field for debugging purposes
        if (SceneManager.GetActiveScene().name != "_MainMenu") {
            chatBroadcaster.ConnectToChannel(chatBroadcaster._channelToConnectTo);
            party.Initialize(chatBroadcaster._channelToConnectTo);
        }

        this.gameState.Initialize();
    }

}