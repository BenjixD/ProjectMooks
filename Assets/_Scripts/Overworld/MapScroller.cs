using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScroller : MonoBehaviour {
    public RectTransform scrollingTransform = null;
    [SerializeField] private float _scrollSpeed = 0;
    private Vector3 _destVector = Vector3.zero;
    private float _leftBound;
    private float _rightBound;

    public void SetBounds(float left, float right) {
        _leftBound = left;
        _rightBound = right;
    }

    // Centre map on given x
    public void RefocusMap(float newX) {
        Vector3 newPos = scrollingTransform.anchoredPosition;
        newPos.x = newX;
        scrollingTransform.anchoredPosition = newPos;
    }

    private void Update() {
        float scrollDist = Input.mouseScrollDelta.y * _scrollSpeed;
        float clampedX = Mathf.Clamp(scrollingTransform.position.x + scrollDist, _leftBound, _rightBound);
        _destVector = scrollingTransform.position;
        _destVector.x = clampedX;
        scrollingTransform.position = _destVector;
    }
}
