using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QteCommonUI : MonoBehaviour {
    [Header("References (QteUI)")]
    [SerializeField] private TextMeshProUGUI _guidanceText = null;
    [SerializeField] private GameObject _warmup = null;
    [SerializeField] private TextMeshProUGUI _countdownText = null;
    [SerializeField] private TextMeshProUGUI _timerText = null;

    public void SetGuidance(string text) {
        _guidanceText.text = text;
    }

    public void UpdateCountdown(int time) {
        _countdownText.text = time.ToString();
    }

    public void EndWarmup() {
        _warmup.SetActive(false);
        _timerText.gameObject.SetActive(true);
    }

    public void UpdateTimer(int time) {
        _timerText.text = "Time: " + time;
    }

    public void DeactivateTimer() {
        _timerText.gameObject.SetActive(false);
    }
}
