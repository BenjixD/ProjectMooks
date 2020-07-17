using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionQTE : QuickTimeEvent {
    [Header("Explosion Parameters")]
    [SerializeField] private char _increaseChar = '+';
    [SerializeField] private char _decreaseChar = '-';
    [SerializeField, Tooltip("Character count beyond which characters offer no additional value.")]
    private int _cutoffLength = 0;
    [SerializeField, Tooltip("Function returning flat percent change given number of input characters. Normalized for messages of count 1 to cutoffLength.")]
    private AnimationCurve _messageValue = null;
    [SerializeField, Range(0, 1), Tooltip("Starting power of the explosion measured in percent.")]
    private float _startingPower = 0;
    private float _explosionPower = 0;

    [Header("References (ExplosionQTE)")]
    [SerializeField, Tooltip("Reference to the ExplosionQTECanvas prefab.")]
    private GameObject _explosionCanvasPrefab = null;
    private ExplosionUI _explosionUI;

    private Explosion _action;

    public void Initialize(FightingEntity user, List<FightingEntity> targets, Explosion action) {
        Initialize(user, targets);
        _action = action;
    }

    protected override void ProcessMessage(string message) {
        _explosionPower += CalculateValue(message);
        _explosionPower = Mathf.Clamp(_explosionPower, 0, 1);
        UpdatePowerMeter();
    }

    protected override void OpenInput() {
        base.OpenInput();
        _explosionUI = Instantiate(_explosionCanvasPrefab).GetComponent<ExplosionUI>();
        _explosionPower = _startingPower;
        UpdatePowerMeter();
    }

    private float CalculateValue(string message) {
        int netCount = 0;
        foreach (char c in message) {
            if (c == _increaseChar) {
                netCount++;
            } else if (c == _decreaseChar) {
                netCount--;
            }
        }
        if (netCount == 0) {
            return 0;
        }
        float input = Mathf.InverseLerp(1, _cutoffLength, Mathf.Abs(netCount));
        float value = _messageValue.Evaluate(input);
        if (netCount < 0) {
            value = -value;
        }
        return value;
    }

    private void UpdatePowerMeter() {
        int rawDamage = 0;
        if (_user == null || _action == null) {
            Debug.LogWarning("Explosion damage can't be approximated as user stats or the Explosion action are missng.");
        } else {
            rawDamage = _action.GetRawDamage(_user.stats, _explosionPower);
        }
        _explosionUI.UpdatePower(_explosionPower, rawDamage);
    }

    protected override void ExecuteEffect() {
        _action.FinishQTE(_user, _targets, _explosionPower);
    }

    protected override void DestroyUI() {
        base.DestroyUI();
        Destroy(_explosionUI.gameObject);
    }
}
