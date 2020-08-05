using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {
    [Tooltip("Array of event prefabs. Put into a dictionary on awake.")]
    [SerializeField] private EventStage[] _eventPrefabs = null;
    private Dictionary<EventType, EventStage> _eventDict = new Dictionary<EventType, EventStage>();

    private void Awake() {
        // Populate event dict
        foreach (EventStage eventStage in _eventPrefabs) {
            _eventDict.Add(eventStage.eventType, eventStage);
        }
    }

    public void LoadEvent(EventType eventType) {
        GameObject eventPrefab = GetEvent(eventType).gameObject;
        if (eventPrefab != null) {
            Instantiate(eventPrefab);
        }
    }

    public EventStage GetEvent(EventType eventType) {
        if (!_eventDict.ContainsKey(eventType)) {
            Debug.Log("Requested event prefab is missing");
            return null;
        }
        return _eventDict[eventType];
    }
}
