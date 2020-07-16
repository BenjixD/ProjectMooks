using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MookActionItemUI : MonoBehaviour
{
    public Color unselectedColor;
    public Color selectedColor;
    public TextMeshProUGUI moveNumberText;
    public TextMeshProUGUI actionText;
    public TextMeshProUGUI resourceText;

    public Image background;

    public ActionBase action {get; set;}

    public void Initialize(ActionBase action, int index) {
        this.action = action;
        this.SetText(this.action.name);
        int displayIndex = (index + 1);
        this.moveNumberText.SetText(displayIndex + ": " + action.GetCommandFormat());
        if (action.CostsPP()) {
            resourceText.gameObject.SetActive(true);
            resourceText.SetText(action.currPP.ToString());
        } else {
            resourceText.gameObject.SetActive(false);
        }
    }

    public void SetSelected(bool isSelected) {
        this.background.color = isSelected ? selectedColor : unselectedColor;
    }

    private void SetText(string text) {
        string finalText = text + " " + action.targetInfo.targetType.ToString();
        actionText.SetText(finalText);
    }
}
