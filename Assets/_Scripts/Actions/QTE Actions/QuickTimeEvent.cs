using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuickTimeEvent : MonoBehaviour {
    [Header("Quick Time Event Parameters")]
    [Tooltip("Duration for which this QTE accepts chat input.")]
    [SerializeField] protected float _inputDuration;
    [Tooltip("Instructional text for this QTE that is displayed to the players.")]
    [TextArea] [SerializeField] private string _guidance;

    [Header("References (QuickTimeEvent)")]
    [Tooltip("Reference to the QteCommonCanvas prefab.")]
    [SerializeField] protected GameObject _qteCommonCanvasPrefab;
    private QteCommonUI _qteUI;

    protected bool _acceptingInput;
    
    protected virtual void Start() {
        // TODO: move to later
        OpenInput();
    }

    public void ReceiveMessage(string message) {
        if (_acceptingInput) {
            ProcessMessage(message);
        }
    }

    protected abstract void ProcessMessage(string message);

    protected IEnumerator BeginTimer() { 
        float remaining = _inputDuration;
        _qteUI = Instantiate(_qteCommonCanvasPrefab).GetComponent<QteCommonUI>();
        _qteUI.SetGuidance(_guidance);
        while (remaining > 0) {
            _qteUI.UpdateTimer((int) remaining);
            yield return new WaitForSeconds(1f);
            remaining -= 1;
        }
        CloseInput();
    }

    protected void OpenInput() {
        _acceptingInput = true;
        StartCoroutine(BeginTimer());
    }

    protected virtual void CloseInput() {
        _acceptingInput = false;
        Destroy(_qteUI.gameObject);
        // TODO: execute move
    }
}
