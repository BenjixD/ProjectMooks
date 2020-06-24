using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewZone", menuName = "Zone")]
public class ZoneProperties : ScriptableObject {
    public new string name;
    [Tooltip("The number of rows on the map (i.e., the max number of choices the player has to choose from during each selection).")]
    public int mapRows;
    [Tooltip("The composition of this zone. Each member specifies the nature of each column's stages.")]
    public StageInfo[] columns;
}
