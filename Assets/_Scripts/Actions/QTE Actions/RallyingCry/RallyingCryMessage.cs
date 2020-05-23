using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RallyingCryMessage : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _text;
    private Color32 _textColour;
    private Color32 _fadedTextColour;
    [SerializeField] private float _lifetime;

    private void Start() {
        _textColour = _text.color;
        _fadedTextColour = _textColour;
        _fadedTextColour.a = 0;
        StartCoroutine(Animate());
    }

    public void SetText(string text) {
        _text.text = text;
    }

    private IEnumerator Animate() {
        // TODO: proper effects
        float timeElapsed = 0;
        while (timeElapsed < _lifetime) {
            _text.color = Color.Lerp(_textColour, _fadedTextColour, timeElapsed / _lifetime);;
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
