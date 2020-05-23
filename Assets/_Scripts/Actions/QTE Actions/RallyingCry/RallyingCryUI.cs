using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RallyingCryUI : MonoBehaviour {
    [Header("References")]
    [Tooltip("Reference to the RallyingCryMessage prefab.")]
    [SerializeField] private GameObject _rallyingCryMessagePrefab;
    [SerializeField] private TextMeshProUGUI _powerText;
    [Tooltip("The max number of messages displayed at once. Any further messages sent won't be displayed until quantity drops below this threshold again.")]
    [SerializeField] private int _maxConcurrentMessages;

    private float _screenHalfWidth;
    private float _screenHalfHeight;

    private void Start() {
        _screenHalfWidth = Screen.width / 2;
        _screenHalfHeight = Screen.height / 2;
        Debug.Log("half screen: " + _screenHalfWidth + ", " + _screenHalfHeight);
    }

    public void DisplayMessage(string message) {
        // TODO: object pooling/count
        if (0 < _maxConcurrentMessages) {
            Vector2 randomPosition = Random.insideUnitCircle * new Vector2(_screenHalfWidth, _screenHalfHeight);
            GameObject msg = Instantiate(_rallyingCryMessagePrefab,
                                        randomPosition,
                                        Quaternion.identity,
                                        transform);
            msg.GetComponent<RallyingCryMessage>().SetText(message);
            msg.GetComponent<RectTransform>().anchoredPosition = randomPosition;
        }
    }

    public void UpdatePower(float power) {
        _powerText.text = "Power:\n" + power;
        Debug.Log("power is now at " + power);
    }
}
