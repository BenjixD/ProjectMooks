using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionQTE : QuickTimeEvent {
    [Header("Explosion Parameters")]
    [SerializeField] private string _increaseString;
    [Tooltip("Flat percent increase in power per increase message.")]
    [SerializeField] private float _increaseValue;
    [SerializeField] private string _decreaseString;
    [Tooltip("Flat percent decrease in power per decrease message.")]
    [SerializeField] private float _decreaseValue;
    [Tooltip("Starting power of the explosion measured in percent.")]
    [Range(0, 1)]
    [SerializeField] private float _startingPower;
    private float _explosionPower = 0;

    [Header("References (ExplosionQTE)")]
    [Tooltip("Reference to the ExplosionQTECanvas prefab.")]
    [SerializeField] private GameObject _explosionCanvasPrefab;
    private ExplosionUI _explosionUI;

    private PlayerStats _userStats;
    private Explosion _explosionAction;

    public void Initialize(PlayerStats userStats, Explosion action) {
        _userStats = userStats;
        _explosionAction = action;
    }

    protected override void ProcessMessage(string message) {
        if (message == _increaseString) {
            _explosionPower += _increaseValue;
        } else if (message == _decreaseString) {
            _explosionPower -= _decreaseValue;
        }
        _explosionPower = Mathf.Clamp(_explosionPower, 0, 1);
        UpdatePowerMeter();
    }

    protected override void OpenInput() {
        base.OpenInput();
        _explosionUI = Instantiate(_explosionCanvasPrefab).GetComponent<ExplosionUI>();
        _explosionPower = _startingPower;
        UpdatePowerMeter();
    }

    private void UpdatePowerMeter() {
        int rawDamage = 0;
        if (_explosionAction == null) {
            Debug.LogWarning("Explosion damage can't be approximated as user stats are missng.");
        } else {
            rawDamage = _explosionAction.GetRawDamage(_userStats, _explosionPower);
        }
        _explosionUI.UpdatePower(_explosionPower, rawDamage);
    }

    protected override void DestroyUI() {
        base.DestroyUI();
        Destroy(_explosionUI.gameObject);
    }
}
