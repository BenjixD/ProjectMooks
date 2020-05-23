using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Parallax : MonoBehaviour
{
	[Header("Camera")]
	public GameObject cam;
	[Header("[0f, 1f] - Higher for more camera follow")]
	public float parallaxEffect;

	[Header("Loop background along Axis")]
	public bool repeatX;
	public bool repeatY;

	[Header("Transform Position")]
    public Vector3 position;
	private Vector3 _size;
    private HashSet<ParallaxMotion> _deltas = new HashSet<ParallaxMotion>();

    public void Register(ParallaxMotion delta) {
        _deltas.Add(delta);
    }

    public void Deregister(ParallaxMotion delta) {
        _deltas.Remove(delta);
    }

    void Awake()
    {
        position = transform.position;
        _size = GetComponent<SpriteRenderer>().bounds.size;
    }

    // Update is called once per frame
    void Update()
    {
    	Vector3 cameraMove = cam.transform.position * (1f - parallaxEffect);
    	Vector3 dist = cam.transform.position * parallaxEffect;

        UpdateDelta();
        transform.position = new Vector3(position.x + dist.x, position.y + dist.y, transform.position.z);
        if(repeatX) {
            RepeatHorizontal(cameraMove, dist);
        }
        if(repeatY) {
            RepeatVertical(cameraMove, dist);
        }
    }

    // Update Positions From Deltas
    void UpdateDelta() {
        foreach(ParallaxMotion delta in _deltas) {
            delta.Tick();
        }
    }

    // Repeat Horizontal Background
    void RepeatHorizontal(Vector3 cameraMove, Vector3 dist) {
        if(cameraMove.x > position.x + _size.x) {
        	position += new Vector3(_size.x, 0, 0);
        }
        else if(cameraMove.x < position.x - _size.x) {
        	position -= new Vector3(_size.x, 0, 0);
        }
    }

    // Repeat Vertical Background
    void RepeatVertical(Vector3 cameraMove, Vector3 dist) {
        if(cameraMove.y > position.y + _size.y) {
        	position += new Vector3(0, _size.y, 0);
        }
        else if(cameraMove.y < position.y - _size.y) {
        	position -= new Vector3(0, _size.y, 0);
        }
    }
}
