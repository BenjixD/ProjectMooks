using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaShiftQTE : QuickTimeEvent {
    [Header("Mana Shift Parameters")]
    [SerializeField] private string _increaseString;
    [SerializeField] private float _increaseValue;

    [Header("References (ManaShiftQTE)")]
    [Tooltip("Reference to the ManaShiftCanvas prefab.")]
    [SerializeField] private GameObject _manaShiftCanvasPrefab;
    private ManaShiftUI _manaShiftUI;
    private float _totalPower = 0;

    protected override void ProcessMessage(string message) {
        if (message == _increaseString) {
            _totalPower += _increaseValue;
        }
        UpdatePower();
    }
    
    protected override void OpenInput() {
        base.OpenInput();
        _manaShiftUI = Instantiate(_manaShiftCanvasPrefab).GetComponent<ManaShiftUI>();
        UpdatePower();
    }

    private void UpdatePower() {
        _manaShiftUI.UpdatePower(_totalPower);
    }

    protected override void DestroyUI() {
        base.DestroyUI();
        Destroy(_manaShiftUI.gameObject);
    }
}
