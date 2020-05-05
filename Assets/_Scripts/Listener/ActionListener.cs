using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionListener : TwitchChatListenerBase {
    private Player _player;

    public override void Awake() {
        base.Awake();
        _player = GetComponent<Player>();
    }

    public override void OnMessageReceived(string username, string message) {
        
    }

    public override void OnCommandReceived(string username, string message) {
        _player.TryActionCommand(message);
    }
}
