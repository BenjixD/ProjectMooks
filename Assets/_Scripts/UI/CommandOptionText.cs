using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommandOptionText : MonoBehaviour
{
    public Color selectedColor;
    public Color unselectedColor;

    public Color confirmedColor;

    public TextMeshProUGUI text;
    public Image backgroundImage;
 
    public void SetSelected() {
        this.backgroundImage.color = selectedColor;
    }

    public void SetConfirmed() {
        this.backgroundImage.color = confirmedColor;
    }

    public void SetUnSelected() {
        this.backgroundImage.color = unselectedColor;
    }
}
