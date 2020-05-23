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

    protected override void Start() {
        base.Start();
        _explosionUI = Instantiate(_explosionCanvasPrefab).GetComponent<ExplosionUI>();
        _explosionPower = _startingPower;
        UpdatePowerMeter();
    }

    public void SetUserStats(PlayerStats userStats) {
        _userStats = userStats;
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

    private void UpdatePowerMeter() {
        _explosionUI.UpdatePower(_explosionPower);
    }

    // Return damage based on explosion power and user's stats (without accounting for the targets' resistances)
    private float GetRawDamage() {        
        if (_userStats == null) {
            Debug.LogWarning("Explosion damage can't be approximated as user stats are missng.");
            return 0;
        }

        // TODO: damage calculation
        return _explosionPower * _userStats.GetSpecial();
    }
    
    protected override void CloseInput() {
        base.CloseInput();
        // TODO: destroy at appropriate time
        Destroy(_explosionUI.gameObject);
    }
}
