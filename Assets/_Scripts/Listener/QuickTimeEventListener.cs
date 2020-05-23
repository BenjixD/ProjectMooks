using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickTimeEventListener : TwitchChatListenerBase {
    private QuickTimeEvent _qte;

    private void Start() {
        // TODO: temp
        _qte = GetComponent<QuickTimeEvent>();
    }

    public override void OnMessageReceived(string username, string message) {
        _qte.ReceiveMessage(message);
    }

    public override void OnCommandReceived(string username, string message) {
        // Ignore commands
    }
}
