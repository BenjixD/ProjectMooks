using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaShiftQTE : QuickTimeEvent {
    [Header("Mana Shift Parameters")]
    [SerializeField] private string _increaseString = null;
    [SerializeField] private float _increaseValue = 0;

    [Header("References (ManaShiftQTE)")]
    [SerializeField, Tooltip("Reference to the ManaShiftCanvas prefab.")]
    private GameObject _manaShiftCanvasPrefab = null;
    private ManaShiftUI _manaShiftUI;
    private float _totalPower = 0;

    private ManaShift _action;

    public void Initialize(FightingEntity user, List<FightingEntity> targets, ManaShift action) {
        Initialize(user, targets);
        _action = action;
    }

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
        _manaShiftUI.UpdatePower(_action.GetManaRestored(_user, _totalPower));
    }

    protected override void ExecuteEffect() {
        _action.FinishQTE(_user, _targets, _totalPower);
    }

    protected override void DestroyUI() {
        base.DestroyUI();
        Destroy(_manaShiftUI.gameObject);
    }
}
