using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowUI : MonoBehaviour
{
    public Image arrowImage;

    public Color curArrowColor;

    public void SetColor(Color color) {
        this.curArrowColor = color;
        arrowImage.color = color;
    }
}
