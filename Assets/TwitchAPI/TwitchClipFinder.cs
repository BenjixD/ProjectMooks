using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


/*
// For clips
public class Datum
{
    public string id { get; set; }
    public string url { get; set; }
    public string embed_url { get; set; }
    public string broadcaster_id { get; set; }
    public string broadcaster_name { get; set; }
    public string creator_id { get; set; }
    public string creator_name { get; set; }
    public string video_id { get; set; }
    public string game_id { get; set; }
    public string language { get; set; }
    public string title { get; set; }
    public int view_count { get; set; }
    public DateTime created_at { get; set; }
    public string thumbnail_url { get; set; }
}

public class Pagination
{
    public string cursor { get; set; }
}

public class RootObject
{
    public List<Datum> data { get; set; }
    public Pagination pagination { get; set; }
}

*/

public class TwitchClipFinder : MonoBehaviour
{

    public string channelName = "loltyler1";
    public string clientId = "197e3m1gb4884291ceo84splouonwb";
    int userId = 0;

    void Start() {
        StartCoroutine(getClip());
    }
 
    IEnumerator getClip() {
        yield return StartCoroutine(GetUserId());

        UnityWebRequest www = UnityWebRequest.Get("https://api.twitch.tv/helix/clips?broadcaster_id=" + userId + "&first=5") ;
        www.SetRequestHeader("Client-Id", clientId);
        

        yield return www.SendWebRequest();
 
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            JSONObject obj = new JSONObject(www.downloadHandler.text);
            Debug.Log(obj.ToString());

            JSONObject data = obj["data"];

			foreach(JSONObject clipObject in data.list){
                // Note: there is usually only one object here
                JSONObject embedObject = clipObject["embed_url"];
                Debug.Log("Embed URL: " + embedObject.str);



                Debug.Log("THUMBNAIL HACK");
                
                double offset = -1;
                JSONObject thumbNailObj = clipObject["thumbnail_url"];
                
                
                // Hacky way to get offset: https://clips-media-assets2.twitch.tv/28009905440-offset-40124-preview-480x272.jpg

                string thumbNailURL = thumbNailObj.str;

                int i = thumbNailURL.IndexOf("offset-");
                if (i >= 0) {
                    string rest = thumbNailURL.Substring(i);
                    // offset-40124-preview-480x272.jpg
                    i = rest.IndexOf("-") + 1;
                    if (i >= 0) {
                        rest = rest.Substring(i);
                        int j = rest.IndexOf("-");
                        Debug.Log(rest);

                        if (j >= 0) {
                            string finalOffset = rest.Substring(0, j);
                            Debug.Log("Final offset: " + finalOffset);
                            offset = double.Parse(finalOffset);
                        }
                    
                    }
                } else {
                    Debug.Log("Thumbnail timestamp not found");
                }


                if (offset != -1) {
                    JSONObject videoIdObject = clipObject["video_id"];
                    Debug.Log("TYPE: " + videoIdObject.type + " " + videoIdObject.str);

                    // Deleted vods have empty video ids
                    if (videoIdObject.str != "") {
                        int videoId = int.Parse(videoIdObject.str);
                        Debug.Log("video id: " + videoId);
                        yield return StartCoroutine(GetChatLog(videoId, offset));
                    }
                }



			}
            
        }
    }



/*

// For login information
[System.Serializable]
public class TwitchUserInfo
{
    public string id { get; set; }
    public string login { get; set; }
    public string display_name { get; set; }
    public string type { get; set; }
    public string broadcaster_type { get; set; }
    public string description { get; set; }
    public string profile_image_url { get; set; }
    public string offline_image_url { get; set; }
    public int view_count { get; set; }
}

[System.Serializable]
public class TwitchUserInfoObject
{
    public List<TwitchUserInfo> data { get; set; }
}

*/
    IEnumerator GetUserId() {
        UnityWebRequest www = UnityWebRequest.Get("https://api.twitch.tv/helix/users?login=" + channelName);
        www.SetRequestHeader("Client-Id", clientId);

        yield return www.SendWebRequest();
 
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            JSONObject obj = new JSONObject(www.downloadHandler.text);
            JSONObject data = obj["data"];

            Debug.Log("Array type: " + data.type);

			foreach(JSONObject userInfo in data.list){
                // Note: there is usually only one object here
                JSONObject idObject = userInfo["id"];
                userId = int.Parse(idObject.str);
                Debug.Log("User id: " + userId);
			}
        }
    }


/*
public partial class Temperatures
    {
        [JsonProperty("comments")]
        public Comment[] Comments { get; set; }

        [JsonProperty("_next")]
        public string Next { get; set; }
    }

    public partial class Comment
    {
        [JsonProperty("_id")]
        public Guid Id { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("channel_id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long ChannelId { get; set; }

        [JsonProperty("content_type")]
        public ContentType ContentType { get; set; }

        [JsonProperty("content_id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long ContentId { get; set; }

        [JsonProperty("content_offset_seconds")]
        public double ContentOffsetSeconds { get; set; }

        [JsonProperty("commenter")]
        public Commenter Commenter { get; set; }

        [JsonProperty("source")]
        public Source Source { get; set; }

        [JsonProperty("state")]
        public State State { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }
    }

*/
    IEnumerator GetChatLog(int videoId, double offset) {

        double foundOffsetSeconds = 0;
        string nextCursor = "";
        JSONObject finalCommentsObj = null;


        while (foundOffsetSeconds < offset) {
            UnityWebRequest www;
            
            if (nextCursor == "") {
                www = UnityWebRequest.Get("https://api.twitch.tv/v5/videos/" + videoId + "/comments?content_offset_seconds=" + offset);
            } else {
                www = UnityWebRequest.Get("https://api.twitch.tv/v5/videos/" + videoId + "/comments?cursor=" + nextCursor + "&content_offset_seconds=" + offset);
            }
            
            
            www.SetRequestHeader("Client-Id", clientId);

            yield return www.SendWebRequest();
    
            if(www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            }
            else {


                JSONObject obj = new JSONObject(www.downloadHandler.text);
                // Show results as text
                //Debug.Log(obj.ToString());

                finalCommentsObj = obj["comments"];
                Debug.Log("Next: " + obj["_next"].str);
                nextCursor =  obj["_next"].str;

                if (finalCommentsObj.list.Count > 0) {
                    JSONObject commentObj = finalCommentsObj[0];

                    JSONObject offsetObj= commentObj["content_offset_seconds"];
                 //   Debug.Log(offsetObj.type);
                    foundOffsetSeconds = offsetObj.n;
                  //  Debug.Log("offset seconds: " + foundOffsetSeconds);

                }
            }
        }

        Debug.Log("Final comments object:");
        Debug.Log(finalCommentsObj.ToString());



    }
}
