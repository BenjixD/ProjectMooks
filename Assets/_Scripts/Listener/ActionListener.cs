using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionListener : TwitchChatListenerBase {
    public class ActionListenerResponse {
        public static string ThisTurnAction = "@{0}, (this turn): {1}";
        public static string NextTurnAction = "@{0}, (next turn): {1}";
        public static string InvalidAction = "@{0}: Invalid command!";
        public static string GenericResponse = "@{0}: {1}";
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
            ActionChoiceResult res = _player.TryActionCommand(message);
            switch(res.state) {
                case ActionChoiceResult.State.UNMATCHED:
                    this.EchoMessage(string.Format(ActionListenerResponse.InvalidAction, username));
                    break;
                case ActionChoiceResult.State.QUEUED:
                    if(GameManager.Instance.battleComponents.turnManager.GetPhase().CanInputActions()) {
                        this.EchoMessage(string.Format(ActionListenerResponse.ThisTurnAction, username, res.GetAmalgamatedMessage()));
                    } else {
                        this.EchoMessage(string.Format(ActionListenerResponse.NextTurnAction, username, res.GetAmalgamatedMessage()));
                    }
                    break;
                default:
                    this.EchoMessage(string.Format(ActionListenerResponse.GenericResponse, username, res.GetAmalgamatedMessage()));
                    break;
            }
        }
    }

    public void HandleMessage(string message) {
        if (_player.fighterMessageBox != null) {
            this._player.fighterMessageBox.HandleMessage(_player.GetOrderColor(), _player.Name, message);
        }
    }
}
