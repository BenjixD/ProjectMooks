using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TwitchChatListenerBase : MonoBehaviour, TwitchChatListener
{

    public bool enableReply;

    public virtual void Awake() {
        TwitchChatBroadcaster.Instance.addListener(this);
    }

    public bool IsReplyEnabled() {
        return enableReply;
    }

    public abstract void OnMessageReceived(string username, string message);
    public abstract void OnCommandReceived(string username, string message);

    public void EchoMessage(string message) {
        if(IsReplyEnabled() && TwitchChatBroadcaster.Instance != null) {
            TwitchChatBroadcaster.Instance.Echo(message);
        }
    }

    public void EchoDirectMessage(string name, string message) {
        if(IsReplyEnabled() && TwitchChatBroadcaster.Instance != null) {
            TwitchChatBroadcaster.Instance.Echo("@" + name + " " + message);
        }
    }

    void OnDestroy() {
        if (TwitchChatBroadcaster.Instance != null) {
            TwitchChatBroadcaster.Instance.removeListener(this);
        }
    }

}
