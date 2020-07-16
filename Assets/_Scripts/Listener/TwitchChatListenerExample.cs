using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwitchChatListenerExample : TwitchChatListenerBase
{
    public int id = 0;
    public override void OnMessageReceived(string username, string message) {
        Debug.Log("Message received from listener: " + id + " " + username + " " + message);
    }

    public override void OnCommandReceived(string username, string message) {
        Debug.Log("Command received from listener: " + id + " " + username + " " + message);
    }
}
