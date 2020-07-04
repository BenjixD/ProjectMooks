using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TwitchChatListenerBase : MonoBehaviour, TwitchChatListener
{
    [SerializeField] private bool enableReply;

    public virtual void Awake() {
        TwitchChatBroadcaster.Instance.addListener(this);
    }

    public bool IsReplyEnabled() {
        return enableReply;
    }

    public abstract void OnMessageReceived(string username, string message);
    public abstract void OnCommandReceived(string username, string message);

    protected void EchoMessage(string message) {
        if(IsReplyEnabled() && TwitchChatBroadcaster.Instance != null) {
            TwitchChatBroadcaster.Instance.Echo(message);
        }
    }

    void OnDestroy() {
        if (TwitchChatBroadcaster.Instance != null) {
            TwitchChatBroadcaster.Instance.removeListener(this);
        }
    }

}
