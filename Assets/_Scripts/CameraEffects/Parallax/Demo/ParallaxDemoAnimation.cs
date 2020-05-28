using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ParallaxDemoAnimation : MonoBehaviour {
	public GameObject foreground;
	public GameObject background;
	public GameObject hero;

	[Header("Text Display")]
	public Text uiText;
	public float textSpeed;

	[Header("Whip Pan Speed")]
	public float whipSpeed;
	public float whipDuration;

	void Start() {
		StartCoroutine(demoParallax1());
	}

	public void WhipPanCamera(Vector3 destination) {
		StartCoroutine(whipPan(destination));
	}

	public void PrintSlowText(string text) {
		// Clear current text
		uiText.text = "";
		StartCoroutine(slowText(text));
	}

	public void CallAnimation(GameObject obj, string anim) {
		obj.GetComponent<AnimationController>().SetAnimation(anim, false);
	}

	public void ZoomInAndOutCamera(float zoomAmt, float duration) {
		StartCoroutine(zoomInAndOutOnLayers(zoomAmt, duration));
	}

	IEnumerator demoParallax1() {
		ZoomInAndOutCamera(2f, 1.5f);
		PrintSlowText("Hero Uses Whack!");
		yield return new WaitForSeconds(15 * textSpeed);
		CallAnimation(hero, "attack");
		yield return new WaitForSeconds(1f);
		WhipPanCamera(new Vector3(-20, 0, -10));
		yield return new WaitForSeconds(whipDuration + 1f);
		PrintSlowText("Enemies took 10 damage!");
		yield return new WaitForSeconds(25 * textSpeed);
		WhipPanCamera(new Vector3(0, 0, -10));
		yield return new WaitForSeconds(whipDuration + 1f);

		StartCoroutine(demoParallax1());
		yield return null;
	}

	IEnumerator whipPan(Vector3 destination) {
		float dir = destination.x - background.transform.position.x < 0 ? -1f : 1f;

		background.GetComponent<Kino.Motion>().enabled = true;
		foreground.GetComponent<Camera>().enabled = false;

		// Give Background Motion
		background.GetComponent<Klak.Motion.ConstantMotion>().translationSpeed = whipSpeed * dir;

		yield return new WaitForSeconds(whipDuration);

		// Update Foreground destination
		foreground.transform.position = destination;
		// Remove Background Motion
		background.GetComponent<Klak.Motion.ConstantMotion>().translationSpeed = 0;

		foreground.GetComponent<Camera>().enabled = true;
		background.GetComponent<Kino.Motion>().enabled = false;
	}

	IEnumerator slowText(string text) {
		foreach(char c in text) {
			uiText.text += c;
			yield return new WaitForSeconds(textSpeed);
		}
	}

	IEnumerator zoomInAndOutOnLayers(float zoomAmt, float duration) {
		for(int i = 0; i < 6; i++) {
			foreground.GetComponent<Camera>().orthographicSize -= zoomAmt/6f;
			background.GetComponent<Camera>().orthographicSize -= zoomAmt/6f;
			yield return new WaitForSeconds(0.1f/6f);
		}

		yield return new WaitForSeconds(duration);

		for(int i = 0; i < 6; i++) {
			foreground.GetComponent<Camera>().orthographicSize += zoomAmt/6f;
			background.GetComponent<Camera>().orthographicSize += zoomAmt/6f;
			yield return new WaitForSeconds(0.1f/6f);
		}
	}
}