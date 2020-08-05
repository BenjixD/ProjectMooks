using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShakeStrength {
    NONE,
    WEAK,
    MEDIUM,
    STRONG
}

public class CameraController : MonoBehaviour {
    
    [Header("Shake Settings")]
    [SerializeField] private float _shakeDuration = 0;
    [SerializeField, Tooltip("The magnitude of a MEDIUM strength shake.")]
    private float _shakeMagnitude = 0;
    [SerializeField, Tooltip("The percent difference between each level of shake strength.")]
    private float _strengthDelta = 0.5f;
    private IEnumerator _lastShake;

    public void Shake(ShakeStrength strength) {
        // If shake is ongoing, stop it before starting new one
        if (_lastShake != null) {
            GameManager.Instance.time.GetController().StopCoroutine(_lastShake);
            _lastShake = null;
        }
        _lastShake = PerformShake(strength);
        GameManager.Instance.time.GetController().StartCoroutine(_lastShake);
    }

    private IEnumerator PerformShake(ShakeStrength strength) {
        Vector3 startingPosition = transform.localPosition;
        float elapsedTime = 0;
        float dampenFactor = 1f;
        float magnitude = GetMagnitude(strength);
        while (elapsedTime < _shakeDuration) {
            elapsedTime += GameManager.Instance.time.deltaTime;
            dampenFactor = Mathf.Lerp(1, 0, elapsedTime / _shakeDuration);
            transform.localPosition = startingPosition + (Vector3) Random.insideUnitCircle * magnitude * dampenFactor;
            yield return null;
        }
        transform.localPosition = startingPosition;
        _lastShake = null;
    }

    private float GetMagnitude(ShakeStrength strength) {
        switch (strength) {
            case (ShakeStrength.NONE):
                return 0;
            case (ShakeStrength.WEAK):
                return _shakeMagnitude * (1 - _strengthDelta);
            case (ShakeStrength.MEDIUM):
                return _shakeMagnitude;
            case (ShakeStrength.STRONG):
                return _shakeMagnitude * (1 + _strengthDelta);
        }
        return _shakeMagnitude;
    }
}
