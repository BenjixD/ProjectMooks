using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionQTE : QuickTimeEvent {
    [Header("Explosion Parameters")]
    [SerializeField] private string _increaseString = null;
    [SerializeField, Tooltip("Flat percent increase in power per increase message.")]
    private float _increaseValue = 0;
    [SerializeField, Tooltip("Flat percent decrease in power per decrease message.")]
    private string _decreaseString = null;
    [SerializeField] private float _decreaseValue = 0;
    [SerializeField, Range(0, 1), Tooltip("Starting power of the explosion measured in percent.")]
    private float _startingPower = 0;
    private float _explosionPower = 0;

    [Header("References (ExplosionQTE)")]
    [SerializeField, Tooltip("Reference to the ExplosionQTECanvas prefab.")]
    private GameObject _explosionCanvasPrefab = null;
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
        if (_userStats == null || _explosionAction == null) {
            Debug.LogWarning("Explosion damage can't be approximated as user stats or the Explosion action are missng.");
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
