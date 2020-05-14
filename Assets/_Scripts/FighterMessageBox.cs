using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FighterMessageBox : MonoBehaviour
{
    public Transform speechParent; 
    public Text speechCanvasText;


    public float _chatBubbleAnimationTime = 5f;
    private float _textCounter = 0;
    private bool _isTextAnimating = false;
    private string _textMessage = "";

    private bool isXFlipped = false;

    public void Initialize(bool isXFlipped) {
        speechParent.gameObject.SetActive(false);
        this.isXFlipped = isXFlipped;

        // The messagebox is a special case, where we want the message box itself to be flipped, but not the text itself
        if (this.isXFlipped) {
            Vector3 localScale = speechCanvasText.transform.localScale;
            speechCanvasText.transform.localScale = new Vector3(-Mathf.Abs(localScale.x), localScale.y, localScale.z);
        }
    }

    public void HandleMessage(string message) {
        _textCounter = 0;
        _textMessage = message;

        if (_isTextAnimating == true) {
            // Do nothing
        } else {
            StartCoroutine(displayChatBubble());
        }
    }

    private IEnumerator displayChatBubble() {
        _isTextAnimating = true;
        speechCanvasText.text = _textMessage;
        speechParent.gameObject.SetActive(true);


        while (_textCounter < _chatBubbleAnimationTime) {
            speechCanvasText.text = _textMessage;
            _textCounter += GameManager.Instance.time.deltaTime;
            yield return null;
        }

        speechParent.gameObject.SetActive(false);
        _isTextAnimating = false;
    }
}
