using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
using TMPro;

public class APIController : MonoBehaviour
{

    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI userScore;

    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    private readonly string baseAPIUrl = "http://127.0.0.1:8080/api"; //TODO: Change to a proper URL later

    private void Start()
    {
        usernameText.text = "";
        userScore.text = "";
    }

    public void OnButtonLogin()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        StartCoroutine(SaveUser(username, password));
    }

    public void OnButtonLeaderboard()
    {
        usernameText.text = "Loading...";
        userScore.text = "Loading...";

        StartCoroutine(GetScores());
    }

    IEnumerator SaveUser(string username, string password)
    {
        string scoreURL = baseAPIUrl + "/saveUser/";
        
        Debug.Log("Username: " + username + " Password: " + password);
        
        
        var saveUserPost = new UnityWebRequest(scoreURL, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(username + password);
        saveUserPost.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        saveUserPost.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        saveUserPost.SetRequestHeader("Content-Type", "application/json");

        yield return saveUserPost.SendWebRequest();

        if (saveUserPost.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(saveUserPost.error);
            yield break;
        }
        
    }

    IEnumerator GetScores()
    {
        string scoreURL = baseAPIUrl + "/getScores/";


        UnityWebRequest scoreInfoRequest = UnityWebRequest.Get(scoreURL);

        yield return scoreInfoRequest.SendWebRequest();

        if (scoreInfoRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(scoreInfoRequest.error);
            yield break;
        }



        JSONNode scoreInfo = JSON.Parse(scoreInfoRequest.downloadHandler.text);
        JSONObject scoreJson = scoreInfo.AsArray[0].AsObject;

        Debug.Log(scoreInfo);


        string usernameURL = baseAPIUrl + "getUsername/" + scoreJson["userId"];

        UnityWebRequest getUsernameRequest = UnityWebRequest.Get(usernameURL);

        yield return getUsernameRequest.SendWebRequest();


        string userId = getUsernameRequest.downloadHandler.text;
        string score = scoreJson["score"];

        Debug.Log(userId);
        Debug.Log(score);

        usernameText.text = userId;
        userScore.text = score;
    }
}
