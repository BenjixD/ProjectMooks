using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldspaceToScreenUI : MonoBehaviour {
    [Tooltip("The transform to keep properly positioned.")]
    [SerializeField] private Transform _uiTransform;
    [Tooltip("Optional transform to set the world point to.")]
    [SerializeField] private Transform _worldPointTransform;
    private Vector3 _worldPoint;
    private Camera _cam;

    private void Awake() {
        _cam = Camera.main;
        if (_worldPointTransform != null) {
            _worldPoint = _worldPointTransform.position;
        }
        UpdatePosition();
    }

    private void Update() {
        UpdatePosition();
    }


    public void SetWorldPoint(Vector3 point) {
        _worldPoint = point;
        UpdatePosition();
    }

    private void UpdatePosition() {
        _uiTransform.position = _cam.WorldToScreenPoint(_worldPoint);
    }
}
