using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : FightingEntity,  TwitchChatListener {
	public string playerName;
	public Job job;

    public Canvas speechCanvas; 
    public Text speechCanvasText;

    public bool isHero = false;


    public float _chatBubbleAnimationTime = 5f;
    private float _textCounter = 0;
    private bool _isTextAnimating = false;
    private string _textMessage = "";

    void Start() {
        TwitchChatBroadcaster.Instance.addListener(this);
    }

	public void Initialize(PlayerCreationData data) {
		SetName(data.name);
		SetStats(data.stats);
		SetJob(data.job);
	}

	// Getters / Setters 
	public string GetJobName() {
		return job.ToString();
	}

    public void OnMessageReceived(string username, string message) {
        if (username ==  Name) {
            this.HandleMessage(message);
        }
    }

    public void OnCommandReceived(string username, string message) {
        if (username == Name) {
            this.HandleCommand(message);
        }
    }

    public void HandleCommand(string message) {
        string[] split = message.Split(' ');
        if (split.Length < 1) {
            Debug.Log("Invalid battle command");
            return;
        }

        // Right now, format is: action target
        // action is one of: Attack(A), Magic(M), Defend(D)
        // target is an integer 1, 2, 3...

        string action = split[0];

        int target = 1;

        int.TryParse(split[1], out target);

        switch (action) {
            case "a":
            case "attack":
                command.option = BattleOption.ATTACK;
            break;

            case "magic":
            case "m":
                command.option = BattleOption.MAGIC;
            break;

            case "defend":
            case "d":
                command.option = BattleOption.DEFEND;
            break;

            default:
                Debug.Log("Invalid battle command");
            break;
        }

        if (target <= 0 || target >= GameManager.Instance.enemies.Count) {
            Debug.Log("Invalid target");
            return;
        }

         command.target = target - 1;
    }

    public void HandleMessage(string message) {
        // TODO: this can probably be handled in the Player class.
        _textCounter = 0;
        _textMessage = message;

        if (_isTextAnimating == true) {
            // Do nothing
        } else {
            StartCoroutine(displayChatBubble());
        }
    }


    IEnumerator displayChatBubble() {
        _isTextAnimating = true;
        speechCanvasText.text = _textMessage;
        speechCanvas.gameObject.SetActive(true);


        while (_textCounter < _chatBubbleAnimationTime) {
            speechCanvasText.text = _textMessage;
            _textCounter += Time.deltaTime;
            yield return null;
        }

        speechCanvas.gameObject.SetActive(true);
        _isTextAnimating = false;
    }

    void OnDestroy() {
        TwitchChatBroadcaster.Instance.removeListener(this);
    }

	public void SetName(string name) {
		this.playerName = name;
	}

	public void SetStats(PlayerStats stats) {
		this.stats = stats;
	}

	public void SetJob(Job job) {
		this.job = job;
	}
}