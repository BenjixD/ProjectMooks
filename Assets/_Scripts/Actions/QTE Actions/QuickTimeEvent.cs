using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuickTimeEvent : MonoBehaviour {
    [Header("Quick Time Event Parameters")]
    [SerializeField, Tooltip("Duration for which this QTE accepts chat input.")]
    protected float _inputDuration = 0;
    [SerializeField, TextArea, Tooltip("Instructional text for this QTE that is displayed to the players.")]
    private string _guidance = null;
    [Tooltip("Duration during which players can see the instructional text and prepare to give input.")]
    private float _warmupDuration = 3;
    [Tooltip("Duration to wait after input is closed before executing action, so that players can see the results of the QTE.")]
    private float _windDownDuration = 1;

    [Header("References (QuickTimeEvent)")]
    [SerializeField, Tooltip("Reference to the QteCommonCanvas prefab.")]
    protected GameObject _qteCommonCanvasPrefab;
    private QteCommonUI _qteUI;

    protected bool _acceptingInput;

    protected FightingEntity _user;
    protected List<FightingEntity> _targets;
    
    private void Start() {
        StartCoroutine(StartQTE());
    }

    public void ReceiveMessage(string message) {
        if (_acceptingInput) {
            ProcessMessage(message);
        }
    }

    protected void Initialize(FightingEntity user, List<FightingEntity> targets) {
        _user = user;
        _targets = targets;
    }

    protected abstract void ProcessMessage(string message);

    private IEnumerator StartQTE() {
        GameManager.Instance.time.Pause();
        _qteUI = Instantiate(_qteCommonCanvasPrefab).GetComponent<QteCommonUI>();
        _qteUI.SetGuidance(_guidance);
        StartCoroutine(Countdown());
        _acceptingInput = true;
        yield return new WaitForSeconds(_warmupDuration);
        StartCoroutine(BeginTimer());
    }

    private IEnumerator Countdown() {
        float remaining = _warmupDuration;
        while (remaining > 0) {
            _qteUI.UpdateCountdown((int) remaining);
            yield return new WaitForSeconds(1f);
            remaining -= 1;
        }
        _qteUI.EndWarmup();
    }

    private IEnumerator BeginTimer() {
        float remaining = _inputDuration;
        while (remaining > 0) {
            _qteUI.UpdateTimer((int) remaining);
            yield return new WaitForSeconds(1f);
            remaining -= 1;
        }
        StartCoroutine(EndQTE());
    }

    protected virtual void ExecuteEffect() {

    }

    private IEnumerator EndQTE() {
        _acceptingInput = false;
        _qteUI.DeactivateTimer();
        yield return new WaitForSeconds(_windDownDuration);
        GameManager.Instance.time.UnPause();
        ExecuteEffect();
        DestroyUI();
        Destroy(gameObject);
    }

    protected virtual void DestroyUI() {
        Destroy(_qteUI.gameObject);
    }
}
