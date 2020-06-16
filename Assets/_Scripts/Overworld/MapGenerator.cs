using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour {
    [Header("References")]
    [SerializeField] private MapNavigator _mapNavigator = null;
    [SerializeField] private MapScroller _mapScroller = null;
    [SerializeField] private GameObject _stageNodePrefab = null;
    [SerializeField] private Transform _mapContainer = null;
    
    [Space]
    
    public float horizontalPadding = 0;
    public float verticalPadding = 0;

    public StageNode[][] GenerateZoneMap(ZoneProperties properties) {
        StageNode[][] map = new StageNode[properties.columns.Length + 2][];

        // Initialize columns
        for (int col = 0; col < properties.columns.Length + 2; col++) {
            // Create column of nodes
            map[col] = new StageNode[properties.mapRows];
            for (int row = 0; row < properties.mapRows; row++) {
                map[col][row] = null;
            }
        }

        int middleRow = properties.mapRows / 2;
        
        // Create starting column
        StageNode firstStage = InstantiateStageNode(properties, map, 0, middleRow);
        
        // Create end column (boss)
        StageNode finalStage = InstantiateStageNode(properties, map, properties.columns.Length + 1, middleRow);
        InitializeStage(finalStage, StageCategory.BATTLE);

        // Set first and last stage positions as scrolling bounds
        _mapScroller.SetBounds(firstStage.transform.position.x, finalStage.transform.position.x);

        // Create nodes in between
        for (int col = 1; col < properties.columns.Length + 1; col++) {
            for (int row = 0; row < properties.mapRows; row++) {
                StageNode stage = InstantiateStageNode(properties, map, col, row).GetComponent<StageNode>();

                // TODOL
                // InitializeStage(stage, properties.columns[col - 1]);
            }
        }
        return map;
    }

    private StageNode InstantiateStageNode(ZoneProperties properties, StageNode[][] map, int col, int row) {
        map[col][row] = Instantiate(_stageNodePrefab, _mapContainer).GetComponent<StageNode>();
        Vector2 position = new Vector2(horizontalPadding * col, -verticalPadding * row);

        // Centre the node
        Vector2 offset = new Vector2(horizontalPadding * (properties.columns.Length + 1) / 2, -verticalPadding * (properties.mapRows - 1) / 2);
        position -= offset;

        map[col][row].GetComponent<RectTransform>().anchoredPosition = position;
        map[col][row].GetComponent<Button>().onClick.AddListener ( delegate { _mapNavigator.TryMoveTo(new Coord(col, row)); });
        return map[col][row];
    }

    private void InitializeStage(StageNode stage, StageCategory category) {
        switch (category) {
            case StageCategory.BATTLE:
                // TODO: wave data w/ difficulty scaling
                BattleType battleType = (BattleType) Random.Range(0, System.Enum.GetValues(typeof(BattleType)).Length);
                stage.SetStage(battleType);
                break;
            case StageCategory.EVENT:
                EventType eventType = (EventType) Random.Range(0, System.Enum.GetValues(typeof(EventType)).Length);
                stage.SetStage(eventType);
                break;
            default:
                Debug.LogWarning("No category found when initializing stage.");
                break;
        }
    }
}
