using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineEvent : EventStage {
    [SerializeField] private StatusAilment _physUp = null;
    [SerializeField] private StatusAilment _specUp = null;
    [SerializeField] private StatusAilment _defUp = null;
    [SerializeField] private StatusAilment _resUp = null;

    const string eventDesc = "Offering a prayer at the shrine has {0}.";

    protected override void BeginEvent() {
        Player hero = GameManager.Instance.gameState.playerParty.GetHeroFighter();
        AilmentController ailmentController = hero.ailmentController;

        // 50/50 chance to gain offensive buff or defensive buff
        if (Random.value <= 0.5f) {
            ailmentController.AddStatusAilment(_physUp);
            ailmentController.AddStatusAilment(_specUp);
            _descriptionText.text = string.Format(eventDesc, "empowered you");

        } else {
            ailmentController.AddStatusAilment(_defUp);
            ailmentController.AddStatusAilment(_resUp);
            _descriptionText.text = string.Format(eventDesc, "bolstered your defences");
        }
    }
}
