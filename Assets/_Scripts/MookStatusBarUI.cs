using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MookStatusBarUI : StatusBarUI
{

    public Transform energyParent;

    public GameObject energyTokenPrefab;
    public GameObject emptyEnergyTokenPrefab;

    public MookActionMenuUI actionMenuUI;
    
    private List<GameObject> energyBarTokens = new List<GameObject>();

    public override void SetFighter(FightingEntity fighter) {
        base.SetFighter(fighter);
        this.actionMenuUI.SetActions(fighter.actions);
    }

    public void SetEnergy(int energy, int maxEnergy) {
        this.ClearTokens();

        for (int i = 0; i < energy; i++) {
            this.InstantiateToken(energyTokenPrefab);
        }

        for (int i = energy; i < maxEnergy; i++) {
            this.InstantiateToken(emptyEnergyTokenPrefab);

        }
    }

    private void InstantiateToken(GameObject prefab) {
        GameObject instantiatedObject = Instantiate(prefab);
        instantiatedObject.transform.SetParent(energyParent);
        this.energyBarTokens.Add(instantiatedObject);
    }

    private void ClearTokens() {
        foreach (var token in this.energyBarTokens) {
            Destroy(token);
        }

        this.energyBarTokens.Clear();
    }

}
