using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ZoneProperties {
    [Tooltip("The number of rows on the map (i.e., the max number of choices the player has to choose from during each selection).")]
    public int mapRows;
    [Tooltip("The composition of this zone. Each member specifies the nature of each column's rooms. Boss room is automatically appended.")]
    public RoomCategory[] columns;
}
