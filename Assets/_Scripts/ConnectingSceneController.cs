using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectingSceneController : MonoBehaviour
{

    public string battleScene = "AT_Scene";
    // Start is called before the first frame update
    void Start()
    {
        PlayerStats stats = new PlayerStats();
        // TODO: change this later
        stats.maxHp = 100;
        stats.maxMana = 100;
        stats.maxPhysical = 20;
        stats.maxSpecial = 20;
        stats.maxSpeed = 35;
        stats.maxDefense = 10;
        stats.maxResistance = 10;
        stats.RandomizeStats();
        
        PlayerCreationData heroData = new PlayerCreationData(GameManager.Instance.chatBroadcaster._channelToConnectTo, stats, Job.HERO);
        GameManager.Instance.party.CreatePlayer(heroData, 0);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown) {
            SceneManager.LoadScene(battleScene);
        }
    }
}
