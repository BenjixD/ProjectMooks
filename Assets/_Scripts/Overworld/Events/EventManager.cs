using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {
    [Tooltip("Event prefabs, ordered to match EventTypes.")]
    [SerializeField] private EventStage[] _eventPrefabs = null;

    public void LoadEvent(EventType eventType) {
        GameObject eventPrefab = GetEvent(eventType).gameObject;
        if (eventPrefab != null) {
            Instantiate(eventPrefab);
        }
    }

    public EventStage GetEvent(EventType eventType) {
        int eventIndex = (int) eventType;
        if (eventIndex >= _eventPrefabs.Length) {
            Debug.Log("Requested event prefab is missing");
            return null;
        }
        return _eventPrefabs[eventIndex];
    }
}
