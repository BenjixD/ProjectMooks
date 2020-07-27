using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

public class SoundController : MonoBehaviour {
	// Hack for dictionary in inspector
	public SoundClip[] clips;
	public float volume;
	public AudioSource audioSource;
	private Dictionary<string, List<SoundClip>> _clipsMap;

	void Awake() {
		// Populate _clipsMap
		foreach(SoundClip c in clips) {
			if(!_clipsMap.ContainsKey(c.key)) {
				_clipsMap.Add(c.key, new List<SoundClip>());
			}
			_clipsMap[c.key].Add(c);
		}
	}

	public SoundClip GetRandomClipFromKey(string key) {
		if(_clipsMap.ContainsKey(key)){
			return _clipsMap[key][Random.Range(0, _clipsMap[key].Count - 1)];
		}
		Debug.LogWarning("Trying to get non-existent audioclip: " + key);
		return null;
	}

	public List<SoundClip> GetClipsFromKey(string key) {
		if(_clipsMap.ContainsKey(key)) {
			return _clipsMap[key];	
		}

		Debug.LogWarning("Trying to get non-existent audioclip: " + key);
		return null;
	}

	public void PlayClip(SoundClip clip) {
		audioSource.PlayOneShot(clip.source, volume);
	}
 }