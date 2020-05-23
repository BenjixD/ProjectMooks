using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QteCommonUI : MonoBehaviour {
    [Header("References (QteUI)")]
    [SerializeField] private TextMeshProUGUI _guidanceText;
    [SerializeField] private TextMeshProUGUI _timerText;

    public void SetGuidance(string text) {
        _guidanceText.text = text;
    }

    public void UpdateTimer(int time) {
        _timerText.text = "Time: " + time;
    }
}
