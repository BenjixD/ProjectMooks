using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerQueueUI : MonoBehaviour
{
    public Transform queueParent;
    public BasicText textPrefab;

    public int maxInstantiatedTexts = 10;

    private List<BasicText> texts;

    public void Initialize() {
        texts = new List<BasicText>();
        for (int i = 0; i < maxInstantiatedTexts; i++) {
            BasicText newText = Instantiate<BasicText>(textPrefab, queueParent);
            newText.gameObject.SetActive(false);
            texts.Add(newText);
        }

        Messenger.AddListener<string>(Messages.OnPlayerJoinQueue, this.onPlayerJoinQueue);
        
        this.RefreshUI();
    }

    void OnDestroy() {
        Messenger.RemoveListener<string>(Messages.OnPlayerJoinQueue, this.onPlayerJoinQueue);
    }

    public void RefreshUI() {
        List<PlayerCreationData> playersInQueue = GameManager.Instance.gameState.playerParty.playerQueue.Peek(maxInstantiatedTexts);
        int len = Mathf.Min(playersInQueue.Count, maxInstantiatedTexts);

        for (int i = 0; i < len; i++) {
            PlayerCreationData playerData = playersInQueue[i];
            texts[i].text.SetText(playerData.name);
            texts[i].gameObject.SetActive(true);
        }

        for (int i = len; i < maxInstantiatedTexts; i++) {
            texts[i].gameObject.SetActive(false);
        }
    }

    private void onPlayerJoinQueue(string username) {
        this.RefreshUI();
    }

}