using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManaShiftUI : MonoBehaviour {
    [Header("References")]
    [Tooltip("Reference to the RallyingCryMessage prefab.")]
    [SerializeField] private TextMeshProUGUI _powerText;

    public void UpdatePower(float power) {
        _powerText.text = "Mana Gathered:\n" + power;
    }
}
