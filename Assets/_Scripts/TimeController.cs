using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
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
        GameManager.Instance.time.Pause();
        yield return new WaitForSeconds(seconds);
        GameManager.Instance.time.UnPause();
    } 

    public IEnumerator WaitForSecondsCor(float seconds) {
        float counter = 0;
        while (counter < seconds) {
            counter += GameManager.Instance.time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator WaitForNextFrameCor() {
        yield return null;
    }

    public IEnumerator PausableStartCoroutineCor(IEnumerator cor) {
        while(GameManager.Instance.time.IsPaused()) {
            yield return null;
        }
        yield return base.StartCoroutine(cor);
    }
}
