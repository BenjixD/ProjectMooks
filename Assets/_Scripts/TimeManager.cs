
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeManager : MonoBehaviour
{
    public bool isPaused = false;

    public float deltaTime;


    // Might want to make a base class PauseableBehaviour that extends MonoBehaviour in the future...
    void Update() {
        if (!isPaused) {
            deltaTime = Time.deltaTime;
        } else {
            deltaTime = 0f;
        }
    }

    public Coroutine WaitForSeconds(float seconds) {
        return StartCoroutine(WaitForSecondsCor(seconds));
    }

    public IEnumerator WaitForSecondsCor(float seconds) {
        float counter = 0;
        while (counter < seconds) {
            counter += deltaTime;
            yield return null;
        }
    } 
}