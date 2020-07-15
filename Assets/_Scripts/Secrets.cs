using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Secrets {

    public static string oauthToken;
    public static string clientId;

    public static bool isInitialized = false;

    public static void Initialize() {
        if (isInitialized == true) {
            return;
        }

        TextAsset textAsset = Resources.Load("secrets") as TextAsset;
        string text = textAsset.text;
        // Debug.Log("Secrets: " + text);
        JSONObject obj = new JSONObject(text);

        JSONObject oauthObj = obj["oauth"];

        if (oauthObj) {
            oauthToken = oauthObj.str;
        //    Debug.Log("oAuth token: " + oauthToken);
            isInitialized = true;
        }

        JSONObject clientIDObj = obj["client-id"];

        if (clientIDObj) {
            clientId = clientIDObj.str;
        //    Debug.Log("Client-id: " + clientId);
        }


        if (isInitialized == false) {
            Debug.LogError("Could not get oauth token! Please add Resources/secrets.json");
        }
    }
}
