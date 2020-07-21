using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldspaceToScreenUI : MonoBehaviour {
    [Tooltip("The transform to keep properly positioned.")]
    [SerializeField] private Transform _uiTransform = null;
    [Tooltip("Optional transform to set the world point to.")]
    [SerializeField] private Transform _worldPointTransform = null;
    [Tooltip("Set to true to update _uiTransform's position every frame.")]
    public bool continuouslyUpdate = false;
    private Vector3 _worldPoint = Vector3.zero;
    private Camera _cam;

    private void Awake() {
        _cam = Camera.main;
        if (_worldPointTransform != null) {
            _worldPoint = _worldPointTransform.position;
            UpdatePosition();
        }
    }

    private void Update() {
        if (continuouslyUpdate) {
            UpdatePosition();
        }
    }
    public void SetWorldPoint(Vector3 point) {
        _worldPoint = point;
        if (_cam != null) {
            UpdatePosition();
        }
        SetUpdate(true);
    }

    public void SetUpdate(bool update) {
        continuouslyUpdate = update;
    }


    private void UpdatePosition() {
        _uiTransform.position = _cam.WorldToScreenPoint(_worldPoint);
    }
}
