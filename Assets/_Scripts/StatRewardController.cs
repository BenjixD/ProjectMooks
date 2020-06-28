using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatRewardController : MonoBehaviour
{

    public Transform statRewardUIParent;
    public StatRewardUI statRewardUIPrefab;
    public BasicText currentStatPointsText;

    private Player heroPlayer;

    private List<StatRewardUI> statRewardUIs = new List<StatRewardUI>();

    private Action callback; 

    public void Initialize(Action callback) {
        this.callback = callback;

        this.heroPlayer = GameManager.Instance.gameState.playerParty.GetHeroFighter();

        // TODO: move this somewhere else more appropriate
        this.heroPlayer.stats.LevelUp();

        this.UpdateStatText();
        List<PlayerStatWithModifiers> modifiableStats = this.heroPlayer.stats.GetModifiableStats();
        foreach (PlayerStatWithModifiers stat in modifiableStats) {
            StatRewardUI rewardInstance = Instantiate(statRewardUIPrefab, statRewardUIParent);
            rewardInstance.Initialize(heroPlayer.stats, stat.stat, this.LevelUpStat);
            this.statRewardUIs.Add(rewardInstance);
        }

        StartCoroutine(this.WaitForInput());
    }

    public void LevelUpStat(PlayerStatWithModifiers stat) {
        // Nothing for now
        heroPlayer.stats.LevelUpStat(stat.stat);
        this.UpdateStatText();
        
        foreach (StatRewardUI rewardUI in this.statRewardUIs) {
            rewardUI.UpdateUIValues();
        }

    }

    private void UpdateStatText() {
        this.currentStatPointsText.text.SetText("STAT POINTS: " + heroPlayer.stats.statPoints);
    }


    private IEnumerator WaitForInput() {
        yield return new WaitForSeconds(0.1f); // prevents multi clicking

        while (true) {
            if (Input.GetKeyDown(KeyCode.Z)) {
                this.callback();
                yield break;
            }

            yield return null;
        }
    }

}
