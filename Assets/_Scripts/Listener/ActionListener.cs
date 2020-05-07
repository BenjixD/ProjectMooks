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
        if (username == _player.Name) {
            this.HandleMessage("!" + message);
            _player.TryActionCommand(message);
        }
    }


    public void HandleMessage(string message) {
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

        speechCanvas.gameObject.SetActive(false);
        _isTextAnimating = false;
    }


}
