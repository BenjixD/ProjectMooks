using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


enum CommandCardUIMode {
    SELECT_COMMAND,
    TEXT_ONLY
};
public class CommandCardUI : MonoBehaviour
{
    
    //  SELECT_COMMAND variables ===
    [Header("SELECT COMMAND")]
    // The parent object if mode is SELECT_COMMAND
    [SerializeField]
    private RectTransform _actionMenu;

    // The parent object for texts if mode is SELECT_COMMAND
    [SerializeField]
    private RectTransform _actionMenuTextParent;

    // Prefab for _battleOptionsUI
    [SerializeField]
    private CommandOptionText _commandTextPrefab;

    // The list of text objects if mode is SELECT_COMMAND
    [SerializeField]
    private List<CommandOptionText> _battleOptionsUI {get ; set; }

    // Selection cursor to display user's current selection
    [SerializeField]
    private RectTransform _selectionCursor;
    [SerializeField]
    private Vector2 _selectionCursorOffset = new Vector2(0, 0);

    // TEXT_ONLY variables ===
    // The parent object if mode is TEXT_ONLY
    [Header("TEXT ONLY")]
    [SerializeField]
    private RectTransform _commentaryMenu;

    // The text if mode is TEXT_ONLY
    [SerializeField]
    private Text _commentaryText;



    void Start() {
        _battleOptionsUI = new List<CommandOptionText>();
    }

    public void InitializeCommandSelection(List<string> options) {
        this.clearExistingCommands();

        this.SetCommandCardUIActive(CommandCardUIMode.SELECT_COMMAND);

        foreach (var option in options) {
            CommandOptionText  commandText = Instantiate(_commandTextPrefab) as CommandOptionText;
            commandText.text.text = option;
            commandText.transform.parent = _actionMenuTextParent.transform;
            _battleOptionsUI.Add(commandText);
        }
    }

    public void InitializeCommantaryUI() {
        this.SetCommandCardUIActive(CommandCardUIMode.TEXT_ONLY);
    }

    public void SetCommentaryUIText(string msg) {
        _commentaryText.text = msg;
    }

    public void SetSelectionCursor(int index) {
        if (index < 0 || index >= _battleOptionsUI.Count) {
            return;
        }

        _selectionCursor.transform.parent = _battleOptionsUI[index].transform;
        _selectionCursor.anchoredPosition = _selectionCursorOffset;
    }


    private void SetCommandCardUIActive(CommandCardUIMode mode) {
        switch (mode) {
            case CommandCardUIMode.SELECT_COMMAND:
                _actionMenu.gameObject.SetActive(true);
                _commentaryMenu.gameObject.SetActive(false);
                break;

            case CommandCardUIMode.TEXT_ONLY:
                _actionMenu.gameObject.SetActive(false);
                _commentaryMenu.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    private void clearExistingCommands() {
        for (int i = 0; i < _battleOptionsUI.Count; i++) {
            Destroy(_battleOptionsUI[i].gameObject);
        }
        _battleOptionsUI.Clear();
    }


  

}
