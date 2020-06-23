using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum StageCategory {
    BATTLE,
    BOSS,
    EVENT
}

public enum BattleType {
    EASY_BATTLE,
    MEDIUM_BATTLE,
    HARD_BATTLE,
    BOSS
}

public enum EventType {
    CAMP_GROUNDS,
    LIBRARY
}

public class StageNode : MonoBehaviour {

    public Coord coord;

    [Header("Node Sprites")]
    [SerializeField] private Sprite easyNode = null;
    [SerializeField] private Sprite mediumNode = null;
    [SerializeField] private Sprite hardNode = null;
    [SerializeField] private Sprite eventNode = null;

    [Header("References")]
    [SerializeField] private Image _background = null;
    [SerializeField] private Image _icon = null;
    [SerializeField] private Sprite _battleIcon = null;
    [SerializeField] private Sprite[] _eventIcons = null;

    [Space]

    [SerializeField] private Color _inaccessibleStageColour = Color.black;
    [SerializeField] private string _battleScene = null;
    [SerializeField] private string _eventScene = null;
    private StageCategory _stageCategory;
    // TODOL
    private StageInfoContainer _stageInfoContainer; // Container for StageInfo if this stage is a battle stage
    private EventType _eventType;
    private GameObject _entryBranch;
    private RectTransform _verticalPath;

    public void EnterStage() {
        if (_stageCategory == StageCategory.BATTLE || _stageCategory == StageCategory.BOSS) {
            GameManager.Instance.gameState.progressData.currentStageIndex = GameManager.Instance.gameState.progressData.nextStageIndex;
            // TODOL: load battle based on battle data
            SceneManager.LoadScene(_battleScene);
            GameManager.Instance.gameState.InitializeStage(_stageInfoContainer);
        } else if (_stageCategory == StageCategory.EVENT) {
            // TODOL: pass EventType
            SceneManager.LoadScene(_eventScene);
            GameManager.Instance.eventManager.LoadEvent(_eventType);
        }
    }

    public void SetStage(BattleType type) {
        _stageCategory = StageCategory.BATTLE;
        _icon.sprite = _battleIcon;
        switch(type) {
            case BattleType.EASY_BATTLE:
                _background.sprite = easyNode;
                break;
            case BattleType.MEDIUM_BATTLE:
                _background.sprite = mediumNode;
                break;
            case BattleType.HARD_BATTLE:
                _background.sprite = hardNode;
                break;
            case BattleType.BOSS:
                _background.sprite = hardNode;
                break;
            default:
                Debug.LogWarning("BattleType " + type + " unrecognized");
                break;
        }
    }

    public void SetStage(EventType type) {
        _stageCategory = StageCategory.EVENT;
        _eventType = type;
        _icon.sprite = _eventIcons[(int) type];
        _background.sprite = eventNode;
    }

    public void SetStageInfoContainer(StageInfoContainer stageInfoContainer) {
        _stageInfoContainer = stageInfoContainer;
    }

    public void SetEntryBranch(GameObject branch) {
        _entryBranch = branch;
    }

    public void DestroyEntryBranch() {
        Destroy(_entryBranch);
        _entryBranch = null;
    }
    
    public void SetVerticalPath(RectTransform path) {
        _verticalPath = path;
    }
    
    public RectTransform GetVerticalPath() {
        return _verticalPath;
    }

    public void SetInaccessible() {
        _background.color = _inaccessibleStageColour;
    }
}
