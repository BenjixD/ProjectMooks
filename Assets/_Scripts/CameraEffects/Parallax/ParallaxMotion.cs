using UnityEngine;

[RequireComponent(typeof(Parallax))]
public abstract class ParallaxMotion : MonoBehaviour {
	protected Parallax _parallax;

	void Awake() {
		_parallax = GetComponent<Parallax>();
		_parallax.Register(this);
	}

	// Called on Parallax Update
	public abstract void Tick();
}