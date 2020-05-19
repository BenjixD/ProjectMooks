using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MookActionMenuUI : MonoBehaviour
{
    

    public MookActionItemUI mookActionItemUIPrefab;

    public GameObject emptyActionMenuUIPrefab;

    public Transform actionItemParent;

    private MookActionItemUI[] actions = new MookActionItemUI[4];

    private GameObject[] emptyActions = new GameObject[4];

    public void SetActions(List<ActionBase> actions) {
        if (actions.Count > 4) {
            Debug.LogError("Mooks should have no more than 4 actions");
        } else {

 

            for (int i = 0; i < actions.Count; i++) {
                this.InstantiateEmptyItem(i);
                this.InstantiateActionItem(actions[i], i);
            }

            for (int i = actions.Count; i < 4; i++) {
                this.InstantiateEmptyItem(i);
            }
        }
    }

    public void SetAction(QueuedAction action) {
        ActionBase actionToDo = action._action;
        for (int i = 0; i < actions.Length; i++) {
            if (actions[i] != null) {
                if (actions[i].action == actionToDo) {
                    actions[i].SetSelected(true);
                    emptyActions[i].SetActive(false);
                } else {
                    actions[i].gameObject.SetActive(false);
                    emptyActions[i].SetActive(true);

                }
            }
        }
    }


    private void InstantiateActionItem(ActionBase action, int index) {
        MookActionItemUI menuItem = Instantiate<MookActionItemUI>(mookActionItemUIPrefab);
        menuItem.Initialize(action, index);
        menuItem.transform.SetParent(actionItemParent);
        this.actions[index] = menuItem;
    }

    private void InstantiateEmptyItem(int index) {
        GameObject menuItem = Instantiate(emptyActionMenuUIPrefab);
        menuItem.SetActive(false);
        menuItem.transform.SetParent(actionItemParent);
        this.emptyActions[index] = menuItem;
    }
}
