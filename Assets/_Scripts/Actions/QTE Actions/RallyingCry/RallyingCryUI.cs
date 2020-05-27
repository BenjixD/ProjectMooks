using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RallyingCryUI : MonoBehaviour {
    [Header("References")]
    [SerializeField, Tooltip("Reference to the RallyingCryMessage prefab.")]
    private GameObject _rallyingCryMessagePrefab;
    [SerializeField] private TextMeshProUGUI _powerText;

    [Space]

    [SerializeField, Tooltip("The max number of messages displayed at once. Any further messages will replace oldest messages.")]
    private int _maxConcurrentMessages;
    private List<RallyingCryMessage> _currMessages = new List<RallyingCryMessage>();
    private int _oldestMessageIndex;
    private float _screenHalfWidth;
    private float _screenHalfHeight;

    private void Start() {
        _screenHalfWidth = Screen.width / 2;
        _screenHalfHeight = Screen.height / 2;
        _oldestMessageIndex = 0;
    }

    public void DisplayMessage(string message) {
        if (_currMessages.Count < _maxConcurrentMessages) {
            // Create a new message for the object pool
            RallyingCryMessage msg = Instantiate(_rallyingCryMessagePrefab, transform).GetComponent<RallyingCryMessage>();
            InitializeMessage(msg, message);
            _currMessages.Add(msg);
        } else {
            // If message limit has been reached, change the oldest one instead of making a new one
            InitializeMessage(_currMessages[_oldestMessageIndex], message);
            _oldestMessageIndex = (_oldestMessageIndex + 1) % _maxConcurrentMessages;
        }
    }

    private void InitializeMessage(RallyingCryMessage rallyingCryMessage, string message) {
        Vector2 randomPosition = Random.insideUnitCircle * new Vector2(_screenHalfWidth, _screenHalfHeight);
        rallyingCryMessage.GetComponent<RectTransform>().anchoredPosition = randomPosition;
        rallyingCryMessage.SetText(message);
    }

    public void UpdatePower(float power) {
        _powerText.text = "Power:\n" + power;
    }
}
