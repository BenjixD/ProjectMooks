using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundStartUI : MonoBehaviour
{
    public float textTransitionTime = 0.5f;
    public float textWaitTime = 1f;
    public CanvasGroup textCanvasGroup;

    public void Initialize() {
        this.gameObject.SetActive(false);
    }

	public IEnumerator PlayAnimation(){
        this.gameObject.SetActive(true);
        yield return GameManager.Instance.time.GetController().StartCoroutine(AnimateText());
        this.gameObject.SetActive(false);
	}

   private IEnumerator AnimateText() {
        float currentDisplayCounter = 0;

        while (currentDisplayCounter < this.textTransitionTime) {
            currentDisplayCounter += GameManager.Instance.time.deltaTime;
            this.textCanvasGroup.alpha = currentDisplayCounter / this.textTransitionTime;
            yield return null;
        }

        this.textCanvasGroup.alpha = 1;
        currentDisplayCounter = 0;

        yield return GameManager.Instance.time.GetController().WaitForSecondsCor(this.textWaitTime);

        currentDisplayCounter = 0;

        while (currentDisplayCounter < this.textTransitionTime) {
            currentDisplayCounter += GameManager.Instance.time.deltaTime;
            this.textCanvasGroup.alpha = 1 - (currentDisplayCounter / this.textTransitionTime);
            yield return null;
        }
    }

}
