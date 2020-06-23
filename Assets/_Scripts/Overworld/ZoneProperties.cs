using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewZone", menuName = "Zone")]
public class ZoneProperties : ScriptableObject {
    public new string name;
    // [Tooltip("StageInfo containing information on this zone's battles.")]
    // public StageInfo stageInfo;
    [Tooltip("The number of rows on the map (i.e., the max number of choices the player has to choose from during each selection).")]
    public int mapRows;

    // TODOL: don't make boss automatic
    [Tooltip("The composition of this zone. Each member specifies the nature of each column's stages. Boss stage is automatically appended during generation.")]
    public StageInfo[] columns;

    // public StageInfo2 test;
}
