using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    public InputField channelInputField;
    public Button startButton;
    public Button instructionsButton;
    public Text errorText;

    bool hasChangedChannelText = false;

    bool hasSuccessfullyJoinedChannel = false;


    void Start() {
        Messenger.AddListener(Messages.OnJoinChannel, onJoinChannel);
        Messenger.AddListener(Messages.OnFailedJoinChannel, onFailedJoinChannel);

        errorText.gameObject.SetActive(false);
        channelInputField.interactable = true;
        startButton.interactable = false;
    }

    void OnDestroy() {
        Messenger.RemoveListener(Messages.OnJoinChannel, onJoinChannel);
        Messenger.RemoveListener(Messages.OnFailedJoinChannel, onFailedJoinChannel);
    }


    public void OnPressStart() {
        startButton.interactable = false;
        channelInputField.interactable = false;
        instructionsButton.interactable = false;
        GameManager.Instance.chatBroadcaster.ConnectToChannel(channelInputField.text);
    }

    public void OnInputFieldChanged(string value) {
        errorText.gameObject.SetActive(false);
        startButton.interactable = true;
    }


    private void onJoinChannel() {
        GameManager.Instance.gameState.playerParty.CreateHero(GameManager.Instance.chatBroadcaster._channelToConnectTo);
        SceneManager.LoadScene("WorldMap");
    }

    private void onFailedJoinChannel() {
        channelInputField.interactable = true;
        instructionsButton.interactable = true;
        errorText.gameObject.SetActive(true);
    }
}
