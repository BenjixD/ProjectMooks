using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    public InputField channelInputField;
    public Button startButton;
    public Text errorText;

    bool hasChangedChannelText = false;

    bool hasSuccessfullyJoinedChannel = false;


    void Start() {
        Messenger.AddListener(Messages.OnJoinChannel, onJoinChannel);
        Messenger.AddListener(Messages.OnFailedJoinChannel, onFailedJoinChannel);

        errorText.gameObject.SetActive(false);
        channelInputField.enabled = true;
        startButton.enabled = false;
    }

    void OnDestroy() {
        Messenger.RemoveListener(Messages.OnJoinChannel, onJoinChannel);
        Messenger.RemoveListener(Messages.OnJoinChannel, onFailedJoinChannel);
    }


    public void OnPressStart() {
        startButton.enabled = false;
        channelInputField.enabled = false;
        GameManager.Instance.chatBroadcaster.ConnectToChannel(channelInputField.text);
    }

    public void OnInputFieldChanged(string value) {
        errorText.gameObject.SetActive(false);
        startButton.enabled = true;
    }


    private void onJoinChannel() {
        SceneManager.LoadScene("WorldMap");
    }

    private void onFailedJoinChannel() {
        channelInputField.enabled = true;
        errorText.gameObject.SetActive(true);
    }
}
