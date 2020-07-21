using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class FighterMessageBox : MonoBehaviour
{
    public Transform speechParent; 
    public TextMeshProUGUI speechCanvasUsername;
    public TextMeshProUGUI speechCanvasMessage;


    public float _chatBubbleAnimationTime = 5f;
    private float _textCounter = 0;
    private bool _isTextAnimating = false;
    private Color _nameColour = Color.white;
    private string _username = "";
    private string _textMessage = "";

    private bool isXFlipped = false;

    public void Initialize(bool isXFlipped) {
        speechParent.gameObject.SetActive(false);
        this.isXFlipped = isXFlipped;

        // The messagebox is a special case, where we want the message box itself to be flipped, but not the text itself
        if (this.isXFlipped) {
            Vector3 localScale = speechCanvasMessage.transform.localScale;
            speechCanvasUsername.transform.localScale = new Vector3(-Mathf.Abs(localScale.x), localScale.y, localScale.z);
            speechCanvasMessage.transform.localScale = new Vector3(-Mathf.Abs(localScale.x), localScale.y, localScale.z);
        }
    }

    public void HandleMessage(Color userColour, string username, string message) {
        _textCounter = 0;
        _nameColour = userColour;
        _username = username;
        _textMessage = message;

        if (_isTextAnimating == true) {
            // Do nothing
        } else {
            StartCoroutine(displayChatBubble());
        }
    }

    private IEnumerator displayChatBubble() {
        _isTextAnimating = true;
        speechCanvasUsername.color = _nameColour;
        speechCanvasUsername.text = _username;
        speechCanvasMessage.text = _textMessage;
        speechParent.gameObject.SetActive(true);


        while (_textCounter < _chatBubbleAnimationTime) {
            speechCanvasMessage.text = _textMessage;
            _textCounter += GameManager.Instance.time.deltaTime;
            yield return null;
        }

        speechParent.gameObject.SetActive(false);
        _isTextAnimating = false;
    }
}
