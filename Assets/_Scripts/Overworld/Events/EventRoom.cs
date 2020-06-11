using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventRoom : MonoBehaviour {
    
    [SerializeField] private string _mapScene = "WorldMap";
    [SerializeField] private GameObject _continueButton = null;

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
