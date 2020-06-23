using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManaShiftUI : MonoBehaviour {
    [Header("References")]
    [SerializeField, Tooltip("Reference to the RallyingCryMessage prefab.")]
    private TextMeshProUGUI _powerText = null;

    public void UpdatePower(float power) {
        _powerText.text = power.ToString();
    }
}
