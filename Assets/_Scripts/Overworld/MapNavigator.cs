using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Coord {
    public int col;
    public int row;

    public Coord(int col, int row) {
        this.col = col;
        this.row = row;
    }
    
    public override string ToString() {
        return "Coord(" + col + ", " + row + ")";
    }
}

public class MapNavigator : MonoBehaviour {
    [Header("References")]
    [SerializeField] private MapGenerator _mapGenerator = null;
    [SerializeField] private MapScroller _mapScroller = null;
    [SerializeField] private GameObject _pathPrefab = null;
    [SerializeField] private GameObject _mapDisplay = null;
    [SerializeField] private RectTransform _playerIcon = null;

    private float _horizontalPadding = 0;
    private float _verticalPadding = 0;

    [Space]

    [SerializeField] private string _mapSceneName = null;
    [Tooltip("Maximum distance that the player can move when choosing their path.")]
    [SerializeField] private int _moveRange = 1;


    private List<ZoneProperties> _zoneProperties;
    private int _currZoneIndex = 0;
    private StageNode[][] _currMap;


    private Coord _currCoord;
    private bool _canMove = false;

    private void Awake() {
        if (FindObjectsOfType<MapNavigator>().Length >= 2) {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        _zoneProperties = GameManager.Instance.models.GetZoneProperties();

        _horizontalPadding = _mapGenerator.horizontalPadding;
        _verticalPadding = _mapGenerator.verticalPadding;
    }

    private void Start() {
        GenerateCurrMap();
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == _mapSceneName) {
            OnOpenMap();
        }
    }

    private void GenerateCurrMap() {
        _currMap = _mapGenerator.GenerateZoneMap(_zoneProperties[_currZoneIndex]);
        SetPlayerCoord(0, _zoneProperties[_currZoneIndex].mapRows / 2);
        OnOpenMap();
    }

    private void SetPlayerCoord(int col, int row) {
        _currCoord = new Coord(col, row);
        _playerIcon.SetParent(_currMap[col][row].transform);
        _playerIcon.anchoredPosition = Vector2.zero;
    }

    private void OnOpenMap() {
        EnableNextPaths();
        if (GetCurrStage() != null) {
            float currStageX = GetCurrStage().GetComponent<RectTransform>().anchoredPosition.x;
            _mapScroller.RefocusMap(-(currStageX + _horizontalPadding));
        }
    }

    private void NextMap() {
        if (_currZoneIndex + 1 >= _zoneProperties.Count) {
            Debug.Log("No more zones, game complete");
            return;
        }
        _playerIcon.SetParent(null);
        _mapGenerator.DestroyMap();
        _currZoneIndex++;
        GenerateCurrMap();
    }

    private void SetPlayerCoord(Coord coord) {
        SetPlayerCoord(coord.col, coord.row);
    }

    private StageNode GetCurrStage() { 
        if (_currCoord == null) {
            return null;
        }
        return _currMap[_currCoord.col][_currCoord.row];
    }

    // Creates a path segment going into or out of the given transform (isEntryBranch specifies which)
    private void CreateBranch(StageNode stage, bool isEntryBranch) {
        float branchWidth = _horizontalPadding / 2;
        RectTransform branch = Instantiate(_pathPrefab, stage.transform).GetComponent<RectTransform>();
        branch.sizeDelta = new Vector2(branchWidth + branch.rect.height, branch.rect.height);
        if (isEntryBranch) {
            branch.anchoredPosition -= new Vector2(branchWidth / 2, 0);
            stage.SetEntryBranch(branch.gameObject);
        } else {
            branch.anchoredPosition += new Vector2(branchWidth / 2, 0);
        }
    }

    // Change given path's height so that it extends from stageA to stageB
    private void PositionVerticalPath(RectTransform path, StageNode stageA, StageNode stageB) {
        if (stageA == null || stageB == null) {
            Debug.LogWarning("Cannot position vertical path between null StageNodes");
            return;
        }
        RectTransform rectA = stageA.GetComponent<RectTransform>();
        RectTransform rectB = stageB.GetComponent<RectTransform>();
        path.sizeDelta = new Vector2(path.rect.width, Mathf.Abs(rectA.anchoredPosition.y - rectB.anchoredPosition.y) + path.rect.width);
        // Set absolute y of the path
        float pathY = (rectA.anchoredPosition.y + rectB.anchoredPosition.y) / 2;
        Transform originalParent = path.parent;
        path.SetParent(path.parent.parent);
        path.anchoredPosition = new Vector2(path.anchoredPosition.x, pathY);
        path.SetParent(originalParent);
    }

    private void EnableNextPaths() {
        _mapDisplay.SetActive(true);
        if (_currCoord == null) {
            return;
        }
        if (_currCoord.col + 1 >= _currMap.Length) {
            NextMap();
            return;
        }
        _canMove = true;

        // Create branch leaving current node
        CreateBranch(GetCurrStage(), false);

        // Go through next column and find the topmost and bottommost stages that can be reached
        // Also, grey out unreachable stages
        int topBranchIndex = -1;
        int bottomBranchIndex = -1;
        for (int i = 0; i < _currMap[_currCoord.col + 1].Length; i++) {
            StageNode stage = _currMap[_currCoord.col + 1][i];
            if (stage == null) {
                continue;
            }
            if (i < _currCoord.row - _moveRange || i > _currCoord.row + _moveRange) {
                stage.SetInaccessible();
            } else {
                if (topBranchIndex == -1) {
                    topBranchIndex = i;
                }
                if (bottomBranchIndex == -1 || i > bottomBranchIndex) {
                    bottomBranchIndex = i;
                }
            }
        }

        // Create branches entering each of the next nodes
        for (int i = topBranchIndex; i <= bottomBranchIndex; i++) {
            StageNode stage = _currMap[_currCoord.col + 1][i];
            if (stage != null) {
                CreateBranch(stage, true);
            }
        }

        // Connect all the branches with one vertical path
        StageNode stageA = _currMap[_currCoord.col + 1][topBranchIndex];
        StageNode stageB = _currMap[_currCoord.col + 1][bottomBranchIndex];
        if (_currCoord.row < topBranchIndex) {
            stageA = GetCurrStage();
        } else if (_currCoord.row > bottomBranchIndex) {
            stageB = GetCurrStage();
        }
        RectTransform verticalPath = Instantiate(_pathPrefab, GetCurrStage().transform).GetComponent<RectTransform>();
        verticalPath.anchoredPosition += new Vector2(_horizontalPadding / 2, 0);
        GetCurrStage().SetVerticalPath(verticalPath);
        PositionVerticalPath(verticalPath, stageA, stageB);
    }

    public void TryMoveTo(Coord dest) {
        if (CanMoveTo(dest)) {
            StartCoroutine(Move(dest));
        }
    }

    private bool CanMoveTo(Coord dest) {
        bool nextCol = dest.col == _currCoord.col + 1;
        bool adjacent = dest.row >= _currCoord.row - 1 && dest.row <= _currCoord.row + 1;
        return _canMove && nextCol && adjacent;
    }

    private IEnumerator Move(Coord dest) {
        // Remove branches entering the stages not chosen in the next column and grey them out
        for (int i = 0; i < _currMap[dest.col].Length; i++) {
            StageNode stage = _currMap[dest.col][i];
            // Ignore null stages and the current stage
            if (stage == null || i == dest.row) {
                continue;
            }
            stage.SetInaccessible();
            stage.DestroyEntryBranch();
        }

        // Readjust length of vertical path leading to next column
        PositionVerticalPath(GetCurrStage().GetVerticalPath(), GetCurrStage(), _currMap[dest.col][dest.row]);
        
        SetPlayerCoord(dest);
        _canMove = false;

        // TODO: move animation?
        yield return new WaitForSeconds(1f);
        GetCurrStage().EnterStage();
        _mapDisplay.SetActive(false);
    }
}
