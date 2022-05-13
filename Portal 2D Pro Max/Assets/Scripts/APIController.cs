using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;
using TMPro;

public class APIController : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI userScore;

    public TMP_InputField usernameInputLogin;
    public TMP_InputField passwordInputLogin;
    
    public TMP_InputField usernameInputRegister;
    public TMP_InputField passwordInputRegister;

    public TextMeshProUGUI loggedInText;

    private readonly string baseAPIUrl = "http://127.0.0.1:8080/api"; //TODO: Change to a proper URL later
    private readonly string userInformationPath = Application.persistentDataPath + "/CurrentPlayer.txt";
    private readonly string baseLoginText = "Logged in as";

    private void Start()
    {
        usernameText.text = "";
        userScore.text = "";
    }

    public void Update()
    {
        CheckLogin();
    }


    private void CheckLogin()
    {
        
      
        string[] readText = File.ReadAllLines(userInformationPath);


        for (int i = 0; i < readText.Length; i++)
        {
            if (readText[i].Contains("username:"))
            {
                string username = readText[i].Replace("username:", "");
                loggedInText.SetText(baseLoginText + username);
            }
        }
    }

    public void OnButtonLogin()
    {
        string username = usernameInputLogin.text;
        string password = passwordInputLogin.text;

        StartCoroutine(GetUser(username, password));
    }

    IEnumerator GetUser(string username, string password)
    {
        string getAllUsernamesURL = baseAPIUrl + "/getUsernames";
        string usernames;
        
        UnityWebRequest getUsernamesRequest = UnityWebRequest.Get(getAllUsernamesURL);

        yield return getUsernamesRequest.SendWebRequest();

        if (getUsernamesRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(getUsernamesRequest.error);
            yield break;
        }

        usernames = getUsernamesRequest.downloadHandler.text;
        Debug.Log(getUsernamesRequest.downloadHandler.text);

        if (usernames.Contains(usernameInputLogin.text))
        {
            using (StreamWriter writer = new StreamWriter(userInformationPath))
            {
                writer.WriteLine("UUID: 123489248923748923748932");
                writer.WriteLine("username: Monica");
                writer.WriteLine("password: Monica12345");
            }

            CheckLogin();
        }

        
        
        
        yield break; 
    }


    public void OnButtonRegister()
    {
        string username = usernameInputRegister.text;
        string password = passwordInputRegister.text;

        StartCoroutine(SaveUser(username, password));
    }

    IEnumerator SaveUser(string username, string password)
    {
        string scoreURL = baseAPIUrl + "/saveUser/";

        Debug.Log("Username: " + username + " Password: " + password);


        var saveUserPost = new UnityWebRequest(scoreURL, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(username + password);
        saveUserPost.uploadHandler = (UploadHandler) new UploadHandlerRaw(jsonToSend);
        saveUserPost.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        saveUserPost.SetRequestHeader("Content-Type", "application/json");

        yield return saveUserPost.SendWebRequest();

        if (saveUserPost.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(saveUserPost.error);
            yield break;
        }
    }

    public void OnButtonLeaderboard()
    {
        usernameText.text = "Loading...";
        userScore.text = "Loading...";

        StartCoroutine(GetScores());
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