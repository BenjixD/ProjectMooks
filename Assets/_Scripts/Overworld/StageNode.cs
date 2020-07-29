using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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
    SHRINE
}

public class StageNode : MonoBehaviour {

    public Coord coord;

    [Header("Node Sprites")]
    [SerializeField] private Sprite _easyNode = null;
    [SerializeField] private Sprite _mediumNode = null;
    [SerializeField] private Sprite _hardNode = null;
    [SerializeField] private Sprite _eventNode = null;
    [SerializeField] private Sprite _battleIcon = null;

    [Header("References")]
    [SerializeField] private Image _background = null;
    [SerializeField] private Image _icon = null;
    [SerializeField] private TextMeshProUGUI _infoText = null;

    [Space]

    [SerializeField] private Color _inaccessibleStageColour = Color.black;
    [SerializeField] private string _battleScene = null;
    [SerializeField] private string _eventScene = null;
    private StageCategory _stageCategory;
    private StageInfoContainer _stageInfoContainer; // Container for StageInfo if this stage is a battle stage
    private EventType _eventType; // This stage's EventType, if applicable
    private GameObject _entryBranch;
    private RectTransform _verticalPath;

    public void EnterStage() {
        if (_stageCategory == StageCategory.BATTLE || _stageCategory == StageCategory.BOSS) {
            SceneManager.LoadScene(_battleScene);
            GameManager.Instance.gameState.SetStage(_stageInfoContainer);
        } else if (_stageCategory == StageCategory.EVENT) {
            SceneManager.LoadScene(_eventScene);
            GameManager.Instance.eventManager.LoadEvent(_eventType);
        }
    }

    private void SetIcon(Sprite icon) {
        _icon.enabled = true;
        _icon.sprite = icon;
    }

    public void SetStage(BattleType type) {
        _stageCategory = StageCategory.BATTLE;
        SetIcon(_battleIcon);
        switch(type) {
            case BattleType.EASY_BATTLE:
                _background.sprite = _easyNode;
                break;
            case BattleType.MEDIUM_BATTLE:
                _background.sprite = _mediumNode;
                break;
            case BattleType.HARD_BATTLE:
                _background.sprite = _hardNode;
                break;
            case BattleType.BOSS:
                _background.sprite = _hardNode;
                break;
            default:
                Debug.LogWarning("BattleType " + type + " unrecognized");
                break;
        }
    }

    public void SetStage(EventType type) {
        _stageCategory = StageCategory.EVENT;
        _eventType = type;
        EventStage eventStage = GameManager.Instance.eventManager.GetEvent(type);
        SetIcon(eventStage.icon);
        // TODO: temporary info display, beautify later
        _infoText.gameObject.SetActive(true);
        _infoText.text = eventStage.eventName;
        _background.sprite = _eventNode;
    }

    public void SetStageInfoContainer(StageInfoContainer stageInfoContainer) {
        _stageInfoContainer = stageInfoContainer;

        // Set enemy list
        string listAsText = "";
        foreach (KeyValuePair<string, int> enemy in stageInfoContainer.GetEnemyTally()) {
            // TODO: text display is temporary; switch to images or something later
            listAsText += enemy.Value + "x " + enemy.Key + "\n";
        }
        _infoText.gameObject.SetActive(true);
        _infoText.text = listAsText;
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
