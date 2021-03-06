using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class BattleOrderUI : MonoBehaviour
{
    public Transform turnOrderParent;

    public BasicText turnTextPrefab;

    public GameObject arrowPrefab;

    public TextMeshProUGUI turnOrderHeaderText;

    private List<BasicText> turnTexts;
    private List<GameObject> arrows;

    private int index = 0;

    public void Initialize() {
        this.turnOrderHeaderText.gameObject.SetActive(false);
    }


    public void SetTurnOrder(List<FightingEntity> fighters) {
        this.turnOrderHeaderText.gameObject.SetActive(true);
        turnTexts = new List<BasicText>();
        this.arrows = new List<GameObject>();
        this.index = 0;
        for (int i = 0; i < fighters.Count; i++) {
            FightingEntity fighter = fighters[i];
            BasicText instantiatedText = Instantiate<BasicText>(turnTextPrefab);
            instantiatedText.text.SetText(fighter.Name);
            instantiatedText.text.color = fighter.GetOrderColor();

            
           
            instantiatedText.transform.SetParent(turnOrderParent);
            turnTexts.Add(instantiatedText);

            if (i != fighters.Count - 1) {
                GameObject arrow = Instantiate(arrowPrefab);
                arrow.transform.SetParent(turnOrderParent);
                arrows.Add(arrow);
            }
        }
    }

    public void PopFighter() {
        Destroy(this.turnTexts[index].gameObject);
        if (index < this.arrows.Count) {
            Destroy(this.arrows[index]);
        }

        index++;

        if (index == this.turnTexts.Count) {
            this.ClearList();
            this.turnOrderHeaderText.gameObject.SetActive(false);
        }
    }

    private void ClearList() {
        foreach (var turnText in this.turnTexts) {
            if (turnText != null) {
                Destroy(turnText.gameObject);
            }
        }

        foreach (var arrow in this.arrows) {
            if (arrow != null) {
                Destroy(arrow.gameObject);
            }
        }

        this.arrows.Clear();
        this.turnTexts.Clear();
    }

}