using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExplosionUI : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Image _meterContent = null;
    [SerializeField] private TextMeshProUGUI _damageText = null;

    public void UpdatePower(float powerPercent, int damage) {
        _meterContent.fillAmount = powerPercent;
        _damageText.text = damage.ToString();
    }
}
