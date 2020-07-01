using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventStage : MonoBehaviour {
    public string eventName;
    [Tooltip("Map icon for this event.")]
    public Sprite icon;
    [SerializeField] private GameObject _continueButton = null;
    private string _mapScene = "WorldMap";

    private void Start() {
        ProcessEvent();
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

    public void ReturnToMap() {
        SceneManager.LoadScene(_mapScene);
        Destroy(gameObject);
    }
}
