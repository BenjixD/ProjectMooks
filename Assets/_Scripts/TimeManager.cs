
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TimeManager : MonoBehaviour
{
    public float deltaTime;

    [SerializeField] private bool isPaused = false;

    [SerializeField] private TimeController timeControllerPrefab;

    private TimeController controller = null;

    // Might want to make a base class PauseableBehaviour that extends MonoBehaviour in the future...
    void Update() {
        if (!isPaused) {
            deltaTime = Time.deltaTime;
        } else {
            deltaTime = 0f;
        }
    }

    public void Pause() {
        isPaused = true;
    }

    public void UnPause() {
        isPaused = false;
    }

    public bool IsPaused() {
        return isPaused;
    }

    public TimeController GetController() {
        if (controller == null) {
            controller = Instantiate(timeControllerPrefab);
        }

        return this.controller;
    }
}