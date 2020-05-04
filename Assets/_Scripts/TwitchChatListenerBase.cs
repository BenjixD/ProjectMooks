using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TwitchChatListenerBase : MonoBehaviour, TwitchChatListener
{
    public void Awake() {
        TwitchChatBroadcaster.Instance.listeners.Add(this);
    }

    public abstract void OnMessageReceived(string username, string message);

    public abstract void OnCommandReceived(string username, string message);


}
