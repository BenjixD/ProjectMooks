using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManaShiftUI : MonoBehaviour {
    [Header("References")]
    [SerializeField, Tooltip("Reference to the RallyingCryMessage prefab.")]
    private TextMeshProUGUI _powerText;

    public void UpdatePower(float power) {
        _powerText.text = "Mana Gathered:\n" + power;
    }
}
