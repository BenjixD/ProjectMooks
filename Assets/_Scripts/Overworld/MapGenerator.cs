using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour {
    [Header("References")]
    [SerializeField] private MapNavigator _mapNavigator = null;
    [SerializeField] private MapScroller _mapScroller = null;
    [SerializeField] private GameObject _roomNodePrefab = null;
    [SerializeField] private Transform _mapContainer = null;
    
    [Space]
    
    public float horizontalPadding = 0;
    public float verticalPadding = 0;

    public RoomNode[][] GenerateZoneMap(ZoneProperties properties) {
        RoomNode[][] map = new RoomNode[properties.columns.Length + 2][];

        // Initialize columns
        for (int col = 0; col < properties.columns.Length + 2; col++) {
            // Create column of nodes
            map[col] = new RoomNode[properties.mapRows];
            for (int row = 0; row < properties.mapRows; row++) {
                map[col][row] = null;
            }
        }

        int middleRow = properties.mapRows / 2;
        
        // Create starting column
        RoomNode firstRoom = InstantiateRoomNode(properties, map, 0, middleRow);
        
        // Create end column (boss)
        RoomNode finalRoom = InstantiateRoomNode(properties, map, properties.columns.Length + 1, middleRow);
        InitializeRoom(finalRoom, RoomCategory.BATTLE);

        // Set first and last room positions as scrolling bounds
        _mapScroller.SetBounds(firstRoom.transform.position.x, finalRoom.transform.position.x);

        // Create nodes in between
        for (int col = 1; col < properties.columns.Length + 1; col++) {
            for (int row = 0; row < properties.mapRows; row++) {
                RoomNode room = InstantiateRoomNode(properties, map, col, row).GetComponent<RoomNode>();
                InitializeRoom(room, properties.columns[col - 1]);
            }
        }
        return map;
    }

    private RoomNode InstantiateRoomNode(ZoneProperties properties, RoomNode[][] map, int col, int row) {
        map[col][row] = Instantiate(_roomNodePrefab, _mapContainer).GetComponent<RoomNode>();
        Vector2 position = new Vector2(horizontalPadding * col, -verticalPadding * row);

        // Centre the node
        Vector2 offset = new Vector2(horizontalPadding * (properties.columns.Length + 1) / 2, -verticalPadding * (properties.mapRows - 1) / 2);
        position -= offset;

        map[col][row].GetComponent<RectTransform>().anchoredPosition = position;
        map[col][row].GetComponent<Button>().onClick.AddListener ( delegate { _mapNavigator.TryMoveTo(new Coord(col, row)); });
        return map[col][row];
    }

    private void InitializeRoom(RoomNode room, RoomCategory category) {
        switch (category) {
            case RoomCategory.BATTLE:
                // TODO: wave data w/ difficulty scaling
                BattleType battleType = (BattleType) Random.Range(0, System.Enum.GetValues(typeof(BattleType)).Length);
                room.SetRoom(battleType);
                break;
            case RoomCategory.EVENT:
                EventType eventType = (EventType) Random.Range(0, System.Enum.GetValues(typeof(EventType)).Length);
                room.SetRoom(eventType);
                break;
            default:
                Debug.LogWarning("No category found when initializing room.");
                break;
        }
    }
}
