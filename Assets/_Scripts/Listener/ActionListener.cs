using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionListener : TwitchChatListenerBase {
    public Canvas speechCanvas; 
    public Text speechCanvasText;


    private Player _player;


    public float _chatBubbleAnimationTime = 5f;
    private float _textCounter = 0;
    private bool _isTextAnimating = false;
    private string _textMessage = "";

    public override void Awake() {
        base.Awake();
        _player = GetComponent<Player>();
    }

    public override void OnMessageReceived(string username, string message) {
        if (username ==  _player.Name) {
            this.HandleMessage(message);
        }
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


    public void HandleMessage(string message) {
        // TODO: this can probably be handled in the Player class.

        if (speechCanvas == null || speechCanvasText == null ) {
            return;
        }

        _textCounter = 0;
        _textMessage = message;

        if (_isTextAnimating == true) {
            // Do nothing
        } else {
            StartCoroutine(displayChatBubble());
        }
    }

    IEnumerator displayChatBubble() {
        _isTextAnimating = true;
        speechCanvasText.text = _textMessage;
        speechCanvas.gameObject.SetActive(true);


        while (_textCounter < _chatBubbleAnimationTime) {
            speechCanvasText.text = _textMessage;
            _textCounter += Time.deltaTime;
            yield return null;
        }

        speechCanvas.gameObject.SetActive(true);
        _isTextAnimating = false;
    }


}
