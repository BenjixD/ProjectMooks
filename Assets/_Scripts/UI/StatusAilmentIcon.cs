using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusAilmentIcon : MonoBehaviour {
    public StatusAilment statusAilment;

    [Header("References")]
    [SerializeField] private Image _icon = null;
    [SerializeField] private TextMeshProUGUI _durationText = null;

    public void SetStatusAilment(StatusAilment ailment) {
        statusAilment = ailment;
        if (ailment.icon != null) {
            _icon.sprite = ailment.icon;
        }
        if (ailment.infiniteDuration) {
            _durationText.text = "";
        } else {
            _durationText.text = ailment.duration.ToString();
        }
    }
}
