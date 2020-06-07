using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestCoroutine : MonoBehaviour {
	void Start() {
		//StartCoroutine(RunCR());
		StartCoroutine(Run1());
		StartCoroutine(Run2());
	}

	IEnumerator RunCR() {
		yield return StartCoroutine(Run1());
		yield return StartCoroutine(Run2());
	}

	IEnumerator Run1() {
		yield return new WaitForSeconds(5f);
		Debug.Log("Hello");
	}

	IEnumerator Run2() {
		yield return new WaitForSeconds(5f);
		Debug.Log("Hello from the other side");
	}
}