﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RallyingCryQTE : QuickTimeEvent {
    [Header("Rallying Cry Parameters")]
    [Tooltip("Maximum value a message can add.")]
    [SerializeField] private float _maxCryValue;
    [Tooltip("The minimum message length that uses the messageValue curve to determine value. Shorter messages are special cases.")]
    [SerializeField] private int _properCryMinLength;
    [Tooltip("Value of each character in a message shorter than properCryMinLength in length.")]
    [SerializeField] private float _shortCryCharValue;
    [Tooltip("Message length beyond which characters offer no additional value.")]
    [SerializeField] private int _cutoffLength;
    [Tooltip("Function returning message value percentages given their lengths. Normalized for messages of length 2 to cutoffLength.")]
    [SerializeField] private AnimationCurve _messageValue;

    [Header("References (RallyingCryQTE)")]
    [Tooltip("Reference to the RallyingCryCanvas prefab.")]
    [SerializeField] private GameObject _rallyingCryCanvasPrefab;
    private RallyingCryUI _rallyingCryUI;

    private float _totalPower = 0;

    protected override void Start() {
        base.Start();
        _rallyingCryUI = Instantiate(_rallyingCryCanvasPrefab).GetComponent<RallyingCryUI>();
        UpdatePower();

    }

    protected override void ProcessMessage(string message) {
        _totalPower += GetMessageValue(message);
        _rallyingCryUI.DisplayMessage(message);
        Debug.Log("processed " + message);
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

    protected override void CloseInput() {
        base.CloseInput();
        // TODO: destroy at appropriate time
        Destroy(_rallyingCryUI.gameObject);
    }
}
