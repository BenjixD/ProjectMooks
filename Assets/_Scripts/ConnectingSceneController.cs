using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectingSceneController : MonoBehaviour
{

    public string battleScene = "AT_Scene";

    public RectTransform partyContainer;

    public List<BasicText> commandOptionTexts;

    // Update is called once per frame
    void Update()
    {
        GameManager.Instance.party.TryFillAllPartySlots();
        List<PlayerCreationData> curPartyMembers = GameManager.Instance.party.GetPlayersPosition();

        for (int i = 0; i < curPartyMembers.Count; i++) {
            commandOptionTexts[i].text.SetText(curPartyMembers[i].name);
            commandOptionTexts[i].gameObject.SetActive(true);
        }

        for (int i = curPartyMembers.Count; i < 4; i++) {
            commandOptionTexts[i].gameObject.SetActive(false);
        }
        

        if (Input.GetKeyUp(KeyCode.Z)) {
            StartCoroutine(changeSceneAfterSeconds(0.1f));
        }
    }

    IEnumerator changeSceneAfterSeconds(float seconds) {
        yield return GameManager.Instance.time.WaitForSeconds(seconds);
        SceneManager.LoadScene(battleScene);
    }
}
