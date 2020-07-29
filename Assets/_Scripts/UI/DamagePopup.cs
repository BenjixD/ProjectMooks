using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum DamageType {
    NORMAL,
    HEALING,
    MANA_RECOVERY
}

public class DamagePopup : MonoBehaviour {
    [Header("References")]
    [SerializeField] private TextMeshProUGUI _damageText = null;
    [SerializeField] private WorldspaceToScreenUI _worldToScreen = null;

    [Header("Popup Properties")]
    [Tooltip("Duration for which UI's alpha is unchanged.")]
    [SerializeField] private float _opaqueTime = 0;
    [Tooltip("Duration of fade animation.")]
    [SerializeField] private float _fadeTime = 0;
    [Tooltip("Speed of text moving up.")]
    [SerializeField] private float _ascensionSpeed = 0;
    [Tooltip("Text colour for healing.")]
    [SerializeField] private Color32 _healColour = Color.white;
    [Tooltip("Text colour for mana recovery.")]
    [SerializeField] private Color32 _manaRecoveryColour = Color.white;

    private Vector3 _worldPosition;

    public void Initialize(FightingEntity target, int damage, DamageType damageType) {
        // Format based on damage type
        if (damageType == DamageType.HEALING) {
            damage = Mathf.Abs(damage);
            _damageText.color = _healColour;
        } else if (damageType == DamageType.MANA_RECOVERY) {
            _damageText.color = _manaRecoveryColour;
        }

        // Set position
        FighterPositions uiPositions = target.GetComponent<FighterPositions>();
        if (uiPositions != null) {
            _worldPosition = uiPositions.damagePopup.position;
        } else {
            Debug.LogWarning(target.name + " has no damagePopupLocation");
        }
        _worldToScreen.SetWorldPoint(_worldPosition);

        SetText(damage);
        GameManager.Instance.time.GetController().StartCoroutine(Fade());
    }

    private void SetText(int damage) {
        _damageText.text = damage.ToString();
    }

    private IEnumerator Fade() {
        float totalLifetime = _opaqueTime + _fadeTime;
        float elapsedTime = 0;
        while (elapsedTime < totalLifetime) {
            float deltaTime = GameManager.Instance.time.deltaTime;
            _worldPosition += new Vector3(0, _ascensionSpeed * deltaTime, 0);
            _worldToScreen.SetWorldPoint(_worldPosition);
            elapsedTime += deltaTime;
            if (elapsedTime > _opaqueTime) {
                float interpolant = Mathf.InverseLerp(_opaqueTime, totalLifetime, elapsedTime);
                Color32 newColour = _damageText.color;
                newColour.a = (byte) Mathf.Lerp(255, 0, interpolant);
                _damageText.color = newColour;
            }
            yield return null;
        }
        Destroy(gameObject);
    }
}
