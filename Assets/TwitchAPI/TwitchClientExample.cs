// If type or namespace TwitchLib could not be found. Make sure you add the latest TwitchLib.Unity.dll to your project folder
// Download it here: https://github.com/TwitchLib/TwitchLib.Unity/releases
// Or download the repository at https://github.com/TwitchLib/TwitchLib.Unity, build it, and copy the TwitchLib.Unity.dll from the output directory
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using UnityEngine;

public class TwitchClientExample : MonoBehaviour
{
	[SerializeField] //[SerializeField] Allows the private field to show up in Unity's inspector. Way better than just making it public
	private string _channelToConnectTo = "dvchibot";

    [SerializeField]
    private string _oauthToken;

	private Client _client;

	private void Start()
	{

        Debug.Log("start");
		// To keep the Unity application active in the background, you can enable "Run In Background" in the player settings:
		// Unity Editor --> Edit --> Project Settings --> Player --> Resolution and Presentation --> Resolution --> Run In Background
		// This option seems to be enabled by default in more recent versions of Unity. An aditional, less recommended option is to set it in code:
		// Application.runInBackground = true;

		//Create Credentials instance
		ConnectionCredentials credentials = new ConnectionCredentials(_channelToConnectTo, _oauthToken);

		// Create new instance of Chat Client
		_client = new Client();

		// Initialize the client with the credentials instance, and setting a default channel to connect to.
		_client.Initialize(credentials, _channelToConnectTo);

		// Bind callbacks to events
		_client.OnConnected += OnConnected;
		_client.OnJoinedChannel += OnJoinedChannel;
		_client.OnMessageReceived += OnMessageReceived;
		_client.OnChatCommandReceived += OnChatCommandReceived;
        _client.OnFailureToReceiveJoinConfirmation += OnFailureToReceiveJoinConfirmation;

		// Connect
		_client.Connect();
	}

    private void OnFailureToReceiveJoinConfirmation(object sender, TwitchLib.Client.Events.OnFailureToReceiveJoinConfirmationArgs e) {
        Debug.Log ("Could not join");
    }

	private void OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
	{
		Debug.Log($"The bot {e.BotUsername} succesfully connected to Twitch.");

		if (!string.IsNullOrWhiteSpace(e.AutoJoinChannel))
			Debug.Log($"The bot will now attempt to automatically join the channel provided when the Initialize method was called: {e.AutoJoinChannel}");
	}

	private void OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
	{
		Debug.Log($"The bot {e.BotUsername} just joined the channel: {e.Channel}");
		_client.SendMessage(e.Channel, "I just joined the channel! PogChamp");
	}

	private void OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
	{
		Debug.Log($"Message received from {e.ChatMessage.Username}: {e.ChatMessage.Message}");
	}

	private void OnChatCommandReceived(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
	{
		switch (e.Command.CommandText)
		{
			case "hello":
				_client.SendMessage(e.Command.ChatMessage.Channel, $"Hello {e.Command.ChatMessage.DisplayName}!");
				break;
			case "about":
				_client.SendMessage(e.Command.ChatMessage.Channel, "I am a Twitch bot running on TwitchLib!");
				break;
			default:
				_client.SendMessage(e.Command.ChatMessage.Channel, $"Unknown chat command: {e.Command.CommandIdentifier}{e.Command.CommandText}");
				break;
		}
	}

	private void Update()
	{
		// Don't call the client send message on every Update,
		// this is sample on how to call the client,
		// not an example on how to code.
		if (Input.GetKeyDown(KeyCode.Space))
		{
            string message = "/w Dvchi I pressed the space key within Unity.";
            Debug.Log("Message sent: " + message);

			_client.SendMessage(_channelToConnectTo, message);
		}
	}
}