using UnityEngine;

public class ConstantParallaxMotion : ParallaxMotion {
	[Header("Translating Direction and Magnitude")]
	public Vector3 translation;

	public override void Tick() {
		var dt = Time.deltaTime;
		_parallax.position += translation * dt;
	}
}