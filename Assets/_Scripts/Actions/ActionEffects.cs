using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionEffects {
    [Tooltip("The damage's elemental aspect.")]
    public Element element;
    [Tooltip("Physical stat scaling. A value of 1 will result in 100% of the user's physical stat being added to the damage.")]
    public float physicalScaling;
    [Tooltip("Special stat scaling. A value of 1 will result in 100% of the user's special stat being added to the damage.")]
    public float specialScaling;
    [Range(0, 1), Tooltip("Percent of max HP to add to effect as physical damage (mitigated by def).")]
    public float mhpPercentPhys;
    [Range(0, 1), Tooltip("Percent of max HP to add to effect as special damage (mitigated by res).")]
    public float mhpPercentSpec;
    [Tooltip("Set to true if this effect can heal the target.")]
    public bool heals = false;
    [Tooltip("Status ailments that may be inflicted.")]
    public List<AilmentInfliction> statusAilments;
}
