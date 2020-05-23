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

    // TODO
    // private System.Action<float> _executionCallback;

    protected override void Start() {

        // TODO: temp
        base.Start();

        _manaShiftUI = Instantiate(_manaShiftCanvasPrefab).GetComponent<ManaShiftUI>();
        UpdatePower();
    }

    // TODO
    // public void SetCallback(System.Action<float> callback) {
    //     _executionCallback = callback;
    // }

    protected override void ProcessMessage(string message) {
        if (message == _increaseString) {
            _totalPower += _increaseValue;
        }
        UpdatePower();
    }

    private void UpdatePower() {
        _manaShiftUI.UpdatePower(_totalPower);
    }

    protected override void CloseInput() {
        base.CloseInput();
        // TODO: destroy at appropriate time
        Destroy(_manaShiftUI.gameObject);
        // _executionCallback(_totalPower);
    }
}
