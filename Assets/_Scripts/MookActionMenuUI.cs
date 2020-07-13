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
            Debug.LogError("Mooks should have no more than 4 actions: " + actions.Count);
        } else {
            for (int i = 0; i < actions.Count; i++) {
                if (emptyActions[i] == null) {
                    this.InstantiateEmptyItem(i);
                }
                if (this.actions[i] == null) {
                    this.InstantiateActionItem(actions[i], i);
                } else {
                    this.actions[i].Initialize(actions[i], i);
                }
            }

            for (int i = actions.Count; i < 4; i++) {
                if (emptyActions[i] == null) {
                    this.InstantiateEmptyItem(i);
                }
            }
        }
    }

    public void SetAction(QueuedAction action) {
        ActionBase actionToDo = action._action;
        this.UnsetActions();
        if (actionToDo == null) {
            return;
        }

        for (int i = 0; i < actions.Length; i++) {
            if (actions[i] != null) {
                if (actions[i].action == actionToDo) {
                    actions[i].SetSelected(true);
                    emptyActions[i].SetActive(false);
                } else {
                    // Not necessary for auto queue (experimental feature)
                    //actions[i].gameObject.SetActive(false);
                    //emptyActions[i].SetActive(true);

                }
            }
        }
    }

    public void UnsetActions() {
        for (int i = 0; i < actions.Length; i++) {
            if (actions[i] != null) {
                emptyActions[i].SetActive(false);
                actions[i].gameObject.SetActive(true);
                actions[i].SetSelected(false);
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
