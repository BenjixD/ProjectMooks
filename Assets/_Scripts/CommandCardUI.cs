using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum CommandCardUIMode {
    SELECT_COMMAND,
    CLOSED
};

[RequireComponent (typeof(TurnController))]
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
    private List<CommandOptionText> _battleOptionsUI {get; set;}

    private CommandSelector _commandSelector {get; set;}

    private List<string> options;

    private int curIndex = 0;

    private CommandCardUIMode cardUIMode;


    public void InitializeCommandSelection(List<string> options, int startChoice, CommandSelector selector) {
        this.options = options;
        this.clearExistingCommands();

        foreach (var option in options) {
            CommandOptionText  commandText = Instantiate(_commandTextPrefab) as CommandOptionText;
            commandText.text.text = option;
            commandText.transform.SetParent(_actionMenuTextParent.transform);
            _battleOptionsUI.Add(commandText);
        }

        _commandSelector = selector;
        _commandSelector.Initialize(0, options.Count - 1, this.SetSelectionFromCommandSelector, true);
        this.curIndex = startChoice;
        this.SetSelection(startChoice);
        this.SetCommandCardUI(CommandCardUIMode.SELECT_COMMAND);
    }

    public void SetCommandCardUI(CommandCardUIMode mode) {
        switch (mode) {
            case CommandCardUIMode.SELECT_COMMAND:
                _battleOptionsUI[curIndex].SetSelected();
                _actionMenu.gameObject.SetActive(true);
                
                break;
            case CommandCardUIMode.CLOSED:
                _actionMenu.gameObject.SetActive(false);
                break;
            default:
                break;
        }

        this.cardUIMode = mode;
    }

    public void SetSelection(int index) {
        if (index < 0 || index >= _battleOptionsUI.Count) {
            return;
        }

        _battleOptionsUI[curIndex].SetUnSelected();
        _battleOptionsUI[index].SetSelected();
        this.curIndex = index;
    }

    public void SetConfirmed() {
        _battleOptionsUI[curIndex].SetConfirmed();
    }

    private void SetSelectionFromCommandSelector() {
        int index = _commandSelector.GetChoice();
        SetSelection( index );
    }

    private void clearExistingCommands() {
        if (_battleOptionsUI != null) {
            for (int i = 0; i < _battleOptionsUI.Count; i++) {
                Destroy(_battleOptionsUI[i].gameObject);
            }
        }

        _battleOptionsUI = new List<CommandOptionText>();
    }
}
