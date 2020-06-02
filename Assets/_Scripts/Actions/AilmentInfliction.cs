using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AilmentInfliction {
    public StatusAilment statusAilment;
    [Tooltip("Leave as 0 to use the status ailment's default duration.")]
    public int duration;
    [Range(0, 1)] public float chance;
}
