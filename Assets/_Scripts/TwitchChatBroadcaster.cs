using TwitchLib.Client.Models;
using TwitchLib.Unity;
using UnityEngine;
using System.Collections.Generic;

// The singleton broadcaster for Twitch-related messages
public class TwitchChatBroadcaster : Singleton<TwitchChatBroadcaster>
{
	[SerializeField]
	public HashSet<TwitchChatListener> listeners = new HashSet<TwitchChatListener>();

	public string _channelToConnectTo = "dvchibot";

	private Client client;

	public void addListener(TwitchChatListener newListener) {
		if (this.listeners.Contains(newListener)) {
			Debug.LogWarning("Warning: Already pushed this listener");
			return;
		}

		this.listeners.Add(newListener);
	}

	public void removeListener(TwitchChatListener removeListener) {
		if (!this.listeners.Contains(removeListener)) {
			Debug.LogWarning("Warning: Listener does not contain");
			return;
		}

		this.listeners.Remove(removeListener);
	}

	public void ConnectToChannel(string channelName)
	{
        this._channelToConnectTo = channelName;
        if (client != null) {
            this.DisconnectClient();
        }


		Secrets.Initialize();

		// To keep the Unity application active in the background, you can enable "Run In Background" in the player settings:
		// Unity Editor --> Edit --> Project Settings --> Player --> Resolution and Presentation --> Resolution --> Run In Background
		// This option seems to be enabled by default in more recent versions of Unity. An aditional, less recommended option is to set it in code:
		// Application.runInBackground = true;

		//Create Credentials instance
		ConnectionCredentials credentials = new ConnectionCredentials(_channelToConnectTo, Secrets.oauthToken);

		// Create new instance of Chat Client
		client = new Client();

		// Initialize the client with the credentials instance, and setting a default channel to connect to.
		client.Initialize(credentials, _channelToConnectTo);

		// Bind callbacks to events
		client.OnConnected += OnConnected;
		client.OnJoinedChannel += OnJoinedChannel;
		client.OnMessageReceived += OnMessageReceived;
        client.OnFailureToReceiveJoinConfirmation += OnFailureToJoinChannel;

		// Connect
		client.Connect();
	}

    public void DisconnectClient() {
        client.Disconnect();
    }

	private void OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
	{
		Debug.Log($"The bot {e.BotUsername} succesfully connected to Twitch.");

		if (!string.IsNullOrWhiteSpace(e.AutoJoinChannel))
			Debug.Log($"The bot will now attempt to automatically join the channel provided when the Initialize method was called: {e.AutoJoinChannel}");
	}

	private void OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
	{
        Messenger.Broadcast(Messages.OnJoinChannel);
		Debug.Log($"The bot {e.BotUsername} just joined the channel: {e.Channel}");
		client.SendMessage(e.Channel, "I just joined the channel! PogChamp");
	}

    private void OnFailureToJoinChannel(object sender, TwitchLib.Client.Events.OnFailureToReceiveJoinConfirmationArgs e) {
        Debug.Log("Failed to join channel: " + _channelToConnectTo);
        Messenger.Broadcast(Messages.OnFailedJoinChannel);
    }

	private void OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
	{
		string username = e.ChatMessage.Username;
		string message = e.ChatMessage.Message;
		Debug.Log("Raw chat message: " + username + " " + message);
		
		bool isCommand = false;
		string command = "";
		if (message[0] == '!') {
			isCommand = true;
			command = message.Substring(1);
			command = command.ToLower();
		}

		foreach (var listener in listeners) {
			if (message.Length == 0) {
				continue;
			}

			if (isCommand) {
				listener.OnCommandReceived(username, command);
			} else {
				listener.OnMessageReceived(username, message);
			}
		}
	}

	public void Whisper(string username, string message) {
		client.SendMessage(_channelToConnectTo, "/w " + username + " " + message);
	}
}
