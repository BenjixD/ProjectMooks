using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {
    [SerializeField] private GameObject[] _eventPrefabs = null;
    private Dictionary<EventType, EventData> _eventDict = null;

    private void Awake() {
        // TODO: process _eventData
    }

    public void LoadEvent(EventType eventType) {
        Instantiate(_eventPrefabs[(int) eventType]);
    }
}
