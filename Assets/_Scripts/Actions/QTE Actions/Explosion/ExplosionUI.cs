using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExplosionUI : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Image _meterContent;
    [SerializeField] private TextMeshProUGUI _powerText;

    public void UpdatePower(float power) {
        _meterContent.fillAmount = power;
        _powerText.text = (power * 100) + "%";
    }
}
