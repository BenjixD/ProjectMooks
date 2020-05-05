using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionListener : TwitchChatListenerBase {
    private Player _player;

    public void SetPlayer(Player player) {
        _player = player;
    }

    public override void OnMessageReceived(string username, string message) {
        
    }

    public override void OnCommandReceived(string username, string message) {
        string[] split = message.Split(' ');
        string actionName = split[0];

        switch (actionName) {
            case "a":
            case "attack":
                // Attack command format: !a [target number]
                _player.TryActionCommand(ActionType.ATTACK, split);
                break;
            case "m":
            case "magic":
                ParseMagicCommand(split);
                break;
            case "d":
            case "defend":
                _player.TryActionCommand(ActionType.DEFEND, split);
                break;
            default:
                Debug.Log("ActionListener received unknown command: " + actionName);
                break;
        }
    }

    private void ParseMagicCommand(string[] split) {
        if (split.Length < 2) {
            Debug.Log("ActionListener received malformed magic command: " + split);
            return;
        }
        ActionType spell;
        switch (split[1]) {
            case "fire1":
                spell = ActionType.FIRE_ONE;
                break;
            case "fire2":
                spell = ActionType.FIRE_ALL;
                break;
            default:
                Debug.Log("ActionListener received malformed magic command: " + split);
                Debug.Log(split[1] + " is not a valid spell name");
                return;
        }
        _player.TryActionCommand(spell, split);
    }
}
