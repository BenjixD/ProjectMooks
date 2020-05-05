using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TwitchChatListenerBase : MonoBehaviour, TwitchChatListener
{
    public void Awake() {
        TwitchChatBroadcaster.Instance.addListener(this);
    }

    public abstract void OnMessageReceived(string username, string message);

    public abstract void OnCommandReceived(string username, string message);


    void OnDestroy() {
        TwitchChatBroadcaster.Instance.removeListener(this);
    }

}
