using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionListener : TwitchChatListenerBase {
    public class ActionListenerResponse {
        public static string ValidAction = "@{0} is using {1}!";
        public static string InvalidAction = "@{0}, invalid move!";
    }

    [SerializeField]
    private PlayerObject _player;

    public override void Awake() {
        base.Awake();
        _player = GetComponent<PlayerObject>();
    }

    public override void OnMessageReceived(string username, string message) {
        if (username ==  _player.Name) {
            this.HandleMessage(message);
        }
    }

    public override void OnCommandReceived(string username, string message) {
        if (username == _player.Name) {
            this.HandleMessage("!" + message);
            if(_player.TryActionCommand(message)) {
                this.EchoMessage(String.Format(ActionListenerResponse.ValidAction, username, message));
            } else {
                this.EchoMessage(String.Format(ActionListenerResponse.InvalidAction, username));
            }
        }
    }


    public void HandleMessage(string message) {
        if (_player.fighterMessageBox != null) {
            this._player.fighterMessageBox.HandleMessage(message);
        }
    }
}
