using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class EventStage : MonoBehaviour {
    public string eventName;
    public EventType eventType;
    [Tooltip("Map icon for this event.")]
    public Sprite icon;
    [SerializeField] protected TextMeshProUGUI _descriptionText = null;
    [SerializeField] protected GameObject _continueButton = null;
    private string _mapScene = "WorldMap";

    private void Start() {
        DontDestroyOnLoad(gameObject);
        InitializeContinueButton();
        ProcessEvent();
    }

    protected virtual void InitializeContinueButton() {
        _continueButton.GetComponent<Button>().onClick.AddListener(ReturnToMap);
    }

    private void ProcessEvent() {
        BeginEvent();
        FinishEvent();
    }

    protected virtual void BeginEvent() {

    }

    private void FinishEvent() {
        _continueButton.SetActive(true);
    }

    private void ReturnToMap() {
        SceneManager.LoadScene(_mapScene);
        Destroy(gameObject);
    }
}
