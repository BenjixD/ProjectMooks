using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class DynamicGridResizer : MonoBehaviour
{
        public int numColumns = 2;
        public int numRows = 2;
     
        // Use this for initialization
        void Start ()
        {
            RectTransform  thisTrans = this.gameObject.GetComponent<RectTransform>();

            float width = thisTrans.rect.width;
            float height = thisTrans.rect.height;
            Vector2 newSize = new Vector2(width / numColumns, height / numRows);
            this.gameObject.GetComponent<GridLayoutGroup>().cellSize = newSize;
        }

}
