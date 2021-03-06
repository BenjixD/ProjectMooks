﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RallyingCryQTE : QuickTimeEvent {
    [Header("Rallying Cry Parameters")]
    [SerializeField, Tooltip("Maximum value a message can add.")]
    private float _maxCryValue = 0;
    [SerializeField, Tooltip("The minimum message length that uses the messageValue curve to determine value. Shorter messages are special cases.")]
    private int _properCryMinLength = 0;
    [SerializeField, Tooltip("Value of each character in a message shorter than properCryMinLength in length.")]
    private float _shortCryCharValue = 0;
    [SerializeField, Tooltip("Message length beyond which characters offer no additional value.")]
    private int _cutoffLength = 0;
    [SerializeField, Tooltip("Function returning message value percentages given their lengths. Normalized for messages of length 2 to cutoffLength.")]
    private AnimationCurve _messageValue = null;

    [Header("References (RallyingCryQTE)")]
    [SerializeField, Tooltip("Reference to the RallyingCryCanvas prefab.")]
    private GameObject _rallyingCryCanvasPrefab = null;
    private RallyingCryUI _rallyingCryUI;
    private float _totalPower = 0;

    private RallyingCry _action;

    public void Initialize(FightingEntity user, List<FightingEntity> targets, RallyingCry action) {
        Initialize(user, targets);
        _action = action;
    }

    protected override void ProcessMessage(string message) {
        _totalPower += GetMessageValue(message);
        _rallyingCryUI.DisplayMessage(message);
        UpdatePower();
    }

    protected override void OpenInput() {
        base.OpenInput();
        _rallyingCryUI = Instantiate(_rallyingCryCanvasPrefab).GetComponent<RallyingCryUI>();
        UpdatePower();
    }

    private void UpdatePower() {
        _rallyingCryUI.UpdatePower(_totalPower);
    }

    // Returns the value of the given message. Messages of length properCryMinLength or more use the animation curve to determine value.
    // Messages shorter than properCryMinLength provide exceptionally low value.
    private float GetMessageValue(string message) {
        int length = message.Length;
        if (length == 0) {
            return 0;
        }
        if (length < _properCryMinLength) {
            return length * _shortCryCharValue;
        }
        return EvaluateCurve(length);
    }

    private float EvaluateCurve(int length) {
        if (length < _properCryMinLength) {
            return 0;
        }
        float input = Mathf.InverseLerp(_properCryMinLength, _cutoffLength, length);
        float percentPower = _messageValue.Evaluate(input);
        return percentPower * _maxCryValue;
    }

    protected override void ExecuteEffect() {
        _action.FinishQTE(_user, _targets, _totalPower);
    }

    protected override void DestroyUI() {
        base.DestroyUI();
        Destroy(_rallyingCryUI.gameObject);
    }
}
