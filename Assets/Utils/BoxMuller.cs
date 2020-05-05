using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMuller {
	public static float GetRandom() {
		float u1 = 1.0f - Random.Range(0.0f, 1.0f);
		float u2 = 1.0f - Random.Range(0.0f, 1.0f);
		float rand = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2) / 4;

		if(rand < -1) {
			rand = -1;
		}
		else if(rand > 1) {
			rand = 1;
		}
		
		return rand;
	}
}