using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionListener : TwitchChatListenerBase {

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
            _player.TryActionCommand(message);
        }
    }


    public void HandleMessage(string message) {
        if (_player.fighterMessageBox != null) {
            this._player.fighterMessageBox.HandleMessage(message);
        }
    }
}
