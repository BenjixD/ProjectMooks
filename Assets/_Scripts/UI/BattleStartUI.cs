using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleStartUI : MonoBehaviour {
    public int segments = 50;
    public float speed = 1000f;

	public float maxRadius = 30000;

	public ParticleSystem particles;

    bool isPlayingRing = false;

    private LineRenderer line;


    public float textTransitionTime = 0.5f;
    public float textWaitTime = 1f;

    public float totalAnimationTime = 1f;


    public CanvasGroup textCanvasGroup;
    private float radius;

    public void Initialize() {
        RectTransform rect = GetComponent<RectTransform> ();
        rect.anchoredPosition = Vector3.zero;
        line = GetComponent<LineRenderer>();
        line.positionCount = (segments + 1);
		line.useWorldSpace = false;

        this.gameObject.SetActive(false);
    }

	public IEnumerator PlayAnimation(){
		gameObject.SetActive (true);
		radius = 100;
		particles.Clear ();
		particles.Play ();

        if (!isPlayingRing) {
            GameManager.Instance.time.GetController().StartCoroutine(AnimateRing());
        } else {
            particles.Clear ();
            particles.Play ();
            this.radius = 100;
        }

        yield return GameManager.Instance.time.GetController().StartCoroutine(AnimateText());
        GameManager.Instance.time.GetController().StartCoroutine(DisableGameObject());
	}

    private IEnumerator DisableGameObject() {
        yield return GameManager.Instance.time.GetController().WaitForSecondsCor(this.totalAnimationTime);
        gameObject.SetActive (false);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            this.PlayAnimation();
        }
    }

	void Reset(){
		radius = 100;
        this.isPlayingRing =false;
	}


    void CreatePoints ()
    {
        float x;
        float y;
		float z = 0;

        float angle = 0f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin (Mathf.Deg2Rad * angle);
            y = Mathf.Cos (Mathf.Deg2Rad * angle);
            line.SetPosition (i,new Vector3(x,y,z) * radius);
            angle -= (360f / segments);
        }
    }

    private IEnumerator AnimateRing() {
        isPlayingRing = true;

        while (radius < maxRadius) {
            CreatePoints();
            radius += speed * GameManager.Instance.time.deltaTime;
            yield return null;
        }

        this.Reset();
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
