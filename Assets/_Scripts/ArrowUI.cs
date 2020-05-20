using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowUI : MonoBehaviour
{
    public Image arrowImage;

    public void SetColor(Color color) {
        arrowImage.color = color;
    }
}
