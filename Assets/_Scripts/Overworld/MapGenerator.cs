using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour {
    [Header("References")]
    [SerializeField] private MapNavigator _mapNavigator = null;
    [SerializeField] private MapScroller _mapScroller = null;
    [SerializeField] private GameObject _stageNodePrefab = null;
    [SerializeField] private RectTransform _mapContainer = null;
    
    [Space]
    
    public float horizontalPadding;
    public float verticalPadding;

    public StageNode[][] GenerateZoneMap(ZoneProperties properties) {
        StageNode[][] map = new StageNode[properties.columns.Length + 1][];

        // Initialize columns
        for (int col = 0; col < properties.columns.Length + 1; col++) {
            // Create column of nodes
            map[col] = new StageNode[properties.mapRows];
            for (int row = 0; row < properties.mapRows; row++) {
                map[col][row] = null;
            }
        }

        int middleRow = properties.mapRows / 2;

        _mapContainer.anchoredPosition = Vector3.zero;
        
        // Create starting column
        StageNode firstStage = InstantiateStageNode(properties, map, 0, middleRow);
        
        // Create end column (boss)
        StageNode finalStage = InstantiateStageNode(properties, map, properties.columns.Length, middleRow);
        InitializeStage(properties, properties.columns.Length, finalStage, StageCategory.BOSS);

        // Set first and last stage positions as scrolling bounds
        _mapScroller.SetBounds(firstStage.transform.position.x, finalStage.transform.position.x);

        // Create nodes in between
        for (int col = 1; col < properties.columns.Length; col++) {
            for (int row = 0; row < properties.mapRows; row++) {
                StageNode stage = InstantiateStageNode(properties, map, col, row).GetComponent<StageNode>();
                InitializeStage(properties, col, stage, properties.columns[col - 1].stageType);
            }
        }
        return map;
    }

    private StageNode InstantiateStageNode(ZoneProperties properties, StageNode[][] map, int col, int row) {
        map[col][row] = Instantiate(_stageNodePrefab, _mapContainer).GetComponent<StageNode>();
        Vector2 position = new Vector2(horizontalPadding * col, -verticalPadding * row);

        // Centre the node
        Vector2 offset = new Vector2(horizontalPadding * (properties.columns.Length) / 2, -verticalPadding * (properties.mapRows - 1) / 2);
        position -= offset;

        map[col][row].GetComponent<RectTransform>().anchoredPosition = position;
        map[col][row].GetComponent<Button>().onClick.AddListener ( delegate { _mapNavigator.TryMoveTo(new Coord(col, row)); });
        return map[col][row];
    }

    private void InitializeStage(ZoneProperties properties, int stageCol, StageNode stage, StageCategory category) {
        switch (category) {
            case StageCategory.BATTLE:
                BattleType battleType = (BattleType) Random.Range(0, System.Enum.GetValues(typeof(BattleType)).Length - 1);
                stage.SetStage(battleType);
                InitializeCombatStage(properties, stageCol, stage, battleType);
                break;
            case StageCategory.BOSS:
                stage.SetStage(BattleType.BOSS);
                InitializeCombatStage(properties, stageCol, stage, BattleType.BOSS);
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

    private void InitializeCombatStage(ZoneProperties properties, int stageCol, StageNode stage, BattleType battleType) {
        StageInfoContainer stageInfo = new StageInfoContainer(properties.columns[stageCol - 1], battleType);
        stage.SetStageInfoContainer(stageInfo);
    }

    public void DestroyMap() {
        int children = _mapContainer.childCount;
        for (int i = children - 1; i >= 0; i--) {
            Destroy(_mapContainer.GetChild(i).gameObject);
        }
    }
}
