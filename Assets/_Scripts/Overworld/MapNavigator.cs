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
}

public class MapNavigator : MonoBehaviour {
    [Header("References")]
    [SerializeField] private MapGenerator _mapGenerator = null;
    [SerializeField] private GameObject _pathPrefab = null;
    [SerializeField] private GameObject _mapDisplay = null;
    [SerializeField] private RectTransform _playerIcon = null;

    private float _horizontalPadding = 0;
    private float _verticalPadding = 0;

    [Space]

    [SerializeField] private string _mapSceneName = null;
    [Tooltip("Maximum distance that the player can move when choosing their path.")]
    [SerializeField] private int _moveRange = 1;

    private RoomNode[][] _currMap;

    // TODO: temp, move
    [SerializeField] private ZoneProperties[] _zoneProperties = null;

    private Coord _currCoord;
    private bool _canMove = false;

    private void Awake() {
        if (FindObjectsOfType<MapNavigator>().Length >= 2) {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        _horizontalPadding = _mapGenerator.horizontalPadding;
        _verticalPadding = _mapGenerator.verticalPadding;
    }

    private void Start() {
        GenerateNewMap();
        EnableNextPaths();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == _mapSceneName) {
            EnableNextPaths();
        }
    }

    private void GenerateNewMap() {
        _currMap = _mapGenerator.GenerateZoneMap(_zoneProperties[0]);
        SetPlayerCoord(0, _zoneProperties[0].mapRows / 2);
    }

    private void SetPlayerCoord(int col, int row) {
        _currCoord = new Coord(col, row);
        _playerIcon.SetParent(_currMap[col][row].transform);
        _playerIcon.anchoredPosition = Vector2.zero;
    }

    private void SetPlayerCoord(Coord coord) {
        SetPlayerCoord(coord.col, coord.row);
    }

    private RoomNode GetCurrRoom() { 
        return _currMap[_currCoord.col][_currCoord.row];
    }

    // Creates a path segment going into or out of the given transform (isEntryBranch specifies which)
    private void CreateBranch(RoomNode room, bool isEntryBranch) {
        float branchWidth = _horizontalPadding / 2;
        RectTransform branch = Instantiate(_pathPrefab, room.transform).GetComponent<RectTransform>();
        branch.sizeDelta = new Vector2(branchWidth + branch.rect.height, branch.rect.height);
        if (isEntryBranch) {
            branch.anchoredPosition -= new Vector2(branchWidth / 2, 0);
            room.SetEntryBranch(branch.gameObject);
        } else {
            branch.anchoredPosition += new Vector2(branchWidth / 2, 0);
        }
    }

    // Change given path's height so that it extends from roomA to roomB
    private void PositionVerticalPath(RectTransform path, RoomNode roomA, RoomNode roomB) {
        if (roomA == null || roomB == null) {
            Debug.LogWarning("Cannot position vertical path between null Rooms");
            return;
        }
        RectTransform rectA = roomA.GetComponent<RectTransform>();
        RectTransform rectB = roomB.GetComponent<RectTransform>();
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
        if (_currCoord == null || _currCoord.col + 1 >= _currMap.Length) {
            Debug.Log("end of map reached, no next rooms");
            return;
        }
        _canMove = true;


        // Create branch leaving current node
        CreateBranch(GetCurrRoom(), false);

        /*
        // Find topmost and bottommost rooms
        int topBranchIndex = Mathf.Max(_currCoord.row - _moveRange, 0);
        while (_currMap[_currCoord.col + 1][topBranchIndex] == null) {
            topBranchIndex++;
        }
        int bottomBranchIndex = Mathf.Min(_currCoord.row + _moveRange, _currMap[_currCoord.col + 1].Length - 1);
        while (_currMap[_currCoord.col + 1][bottomBranchIndex] == null) {
            bottomBranchIndex--;
        }
        */

        // Go through next column and find the topmost and bottommost rooms that can be reached
        // Also, grey out unreachable rooms
        int topBranchIndex = -1;
        int bottomBranchIndex = -1;
        for (int i = 0; i < _currMap[_currCoord.col + 1].Length; i++) {
            RoomNode room = _currMap[_currCoord.col + 1][i];
            if (room == null) {
                continue;
            }
            if (i < _currCoord.row - _moveRange || i > _currCoord.row + _moveRange) {
                room.SetInaccessible();
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
            RoomNode room = _currMap[_currCoord.col + 1][i];
            if (room != null) {
                CreateBranch(room, true);
            }
        }

        // Connect all the branches with one vertical path
        RoomNode roomA = _currMap[_currCoord.col + 1][topBranchIndex];
        RoomNode roomB = _currMap[_currCoord.col + 1][bottomBranchIndex];
        if (_currCoord.row < topBranchIndex) {
            roomA = GetCurrRoom();
        } else if (_currCoord.row > bottomBranchIndex) {
            roomB = GetCurrRoom();
        }
        RectTransform verticalPath = Instantiate(_pathPrefab, GetCurrRoom().transform).GetComponent<RectTransform>();
        verticalPath.anchoredPosition += new Vector2(_horizontalPadding / 2, 0);
        GetCurrRoom().SetVerticalPath(verticalPath);
        PositionVerticalPath(verticalPath, roomA, roomB);
    }

    public void TryMoveTo(Coord dest) {
        if (CanMoveTo(dest)) {
            StartCoroutine(Move(dest));
        }
    }

    private IEnumerator Move(Coord dest) {
        // Remove branches entering the rooms not chosen in the next column and grey them out
        for (int i = 0; i < _currMap[dest.col].Length; i++) {
            RoomNode room = _currMap[dest.col][i];
            // Ignore null rooms and the current room
            if (room == null || i == dest.row) {
                continue;
            }
            room.SetInaccessible();
            room.DestroyEntryBranch();
        }

        // Readjust length of vertical path leading to next column
        PositionVerticalPath(GetCurrRoom().GetVerticalPath(), GetCurrRoom(), _currMap[dest.col][dest.row]);
        
        SetPlayerCoord(dest);
        _canMove = false;

        // TODO: move animation?
        yield return new WaitForSeconds(1f);
        GetCurrRoom().EnterRoom();
        _mapDisplay.SetActive(false);
    }

    private bool CanMoveTo(Coord dest) {
        bool nextCol = dest.col == _currCoord.col + 1;
        bool adjacent = dest.row >= _currCoord.row - 1 && dest.row <= _currCoord.row + 1;
        return _canMove && nextCol && adjacent;
    }
}
