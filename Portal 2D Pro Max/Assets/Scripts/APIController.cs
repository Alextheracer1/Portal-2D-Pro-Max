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

    private readonly string baseAPIUrl = "http://127.0.0.1:8080/"; //TODO: Change to a proper URL later

    private void Start()
    {
        usernameText.text = "";
        userScore.text = "";
    }

    public void OnButtonLeaderboard()
    {
        usernameText.text = "Loading...";
        userScore.text = "Loading...";

        StartCoroutine(GetScores());
    }

    IEnumerator GetScores()
    {
        string scoreURL = baseAPIUrl + "getScores/";
        
        
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
