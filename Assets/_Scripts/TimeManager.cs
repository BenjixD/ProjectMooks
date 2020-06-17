
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeManager : MonoBehaviour
{
    public float deltaTime;

    [SerializeField] private bool isPaused = false;

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

    public Coroutine PauseFor(float seconds)  {
        return base.StartCoroutine(PauseForCor(seconds));
    }

    public new Coroutine StartCoroutine(IEnumerator cor) {
        return base.StartCoroutine(PausableStartCoroutineCor(cor));
    }

    public Coroutine WaitForSeconds(float seconds) {
        return StartCoroutine(WaitForSecondsCor(seconds));
    }

    public Coroutine WaitForNextFrame() {
        return StartCoroutine(WaitForNextFrameCor());
    }

    public IEnumerator PauseForCor(float seconds) {
        Pause();
        yield return new WaitForSeconds(seconds);
        UnPause();
    } 

    public IEnumerator WaitForSecondsCor(float seconds) {
        float counter = 0;
        while (counter < seconds) {
            counter += deltaTime;
            yield return null;
        }
    }

    public IEnumerator WaitForNextFrameCor() {
        yield return null;
    }

    public IEnumerator PausableStartCoroutineCor(IEnumerator cor) {
        while(isPaused) {
            yield return null;
        }
        yield return base.StartCoroutine(cor);
    }
}