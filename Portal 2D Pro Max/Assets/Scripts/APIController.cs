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
using UnityEngine.SceneManagement;

public class APIController : MonoBehaviour
{
    public TextMeshProUGUI userScore;

    public TMP_InputField usernameInputLogin;
    public TMP_InputField passwordInputLogin;

    public TextMeshProUGUI loggedInText;

    public GameObject mainMenu;
    public GameObject loginMenu;

    public Button loginButton;
    public Button logoutButton;

    private readonly string baseAPIUrl = "http://127.0.0.1:8080/api"; //TODO: Change to a proper URL later
    private string _userInformationPath;
    private readonly string baseLoginText = "Logged in as";
    private readonly string baseUUIDTemplate = "UUID: ";
    private readonly string baseUsernameTemplate = "username: ";


    private void Start()
    {
        userScore.text = "";
        _userInformationPath = Application.persistentDataPath + "/CurrentPlayer.txt";
        CheckFile();
    }

    private void CheckFile()
    {
        string[] readText = File.ReadAllLines(_userInformationPath);


        for (int i = 0; i < readText.Length; i++)
        {
            if (readText[i].Contains("username:"))
            {
                string username = readText[i].Replace("username:", "");
                loggedInText.SetText(baseLoginText + username);
                loginButton.gameObject.SetActive(false);
                logoutButton.gameObject.SetActive(true);
            }
            else
            {
                loginButton.gameObject.SetActive(true);
                logoutButton.gameObject.SetActive(false);
            }
        }
    }

    public void OnButtonTry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3, LoadSceneMode.Single);
    }

    public void OnButtonTryYourLuck()
    {
        StartCoroutine(GetCurrentScore());
    }

    public void OnButtonLogout()
    {
        using (StreamWriter writer = new StreamWriter(_userInformationPath))
        {
            writer.WriteLine("empty lol");
        }

        loginButton.gameObject.SetActive(true);
        logoutButton.gameObject.SetActive(false);
        loggedInText.text = "";
    }

    public void OnButtonLogin()
    {
        string username = usernameInputLogin.text;
        string password = passwordInputLogin.text;

        StartCoroutine(GetUser(username, password));
    }

    IEnumerator GetUser(string username, string password)
    {
        string checkLoginURL = baseAPIUrl + "/checkLogin/" + username + "/" + password;

        UnityWebRequest checkLoginRequest = UnityWebRequest.Post(checkLoginURL, "");

        yield return checkLoginRequest.SendWebRequest();

        if (checkLoginRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(checkLoginRequest.error);
            yield break;
        }

        Debug.Log("Result: " + checkLoginRequest.result);
        Debug.Log("ResponseCode: " + checkLoginRequest.responseCode);


        if (username != "" && password != "")
        {
            if (checkLoginRequest.responseCode == 200)
            {
                string getUuidURL = baseAPIUrl + "/getUserId/" + username;

                UnityWebRequest getUuidRequest = UnityWebRequest.Get(getUuidURL);

                yield return getUuidRequest.SendWebRequest();

                if (getUuidRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError(getUuidRequest.error);
                    yield break;
                }

                string uuid = getUuidRequest.downloadHandler.text;

                using (StreamWriter writer = new StreamWriter(_userInformationPath))
                {
                    writer.WriteLine(baseUUIDTemplate + uuid);
                    writer.WriteLine(baseUsernameTemplate + username);
                }

                Debug.Log("Login successful");

                usernameInputLogin.text = "";
                passwordInputLogin.text = "";
                mainMenu.gameObject.SetActive(true);
                loginMenu.gameObject.SetActive(false);
                loginButton.gameObject.SetActive(false);
                logoutButton.gameObject.SetActive(true);
                CheckFile();
            }
            else if (checkLoginRequest.responseCode == 400)
            {
                Debug.Log("Invalid Credentials");
            }
        }
        else
        {
            Debug.LogError("Username and/or Password empty");
        }


        yield break;
    }


    IEnumerator GetCurrentScore()
    {
        string scoreURL = baseAPIUrl + "/getScore/";

        string[] readText = File.ReadAllLines(_userInformationPath);

        for (int i = 0; i < readText.Length; i++)
        {
            if (readText[i].Contains("UUID: "))
            {
                string uuid = readText[i].Replace("UUID: ", "");
                scoreURL += uuid;
                break;
            }
            else
            {
                userScore.text = "0";
                yield break;
            }
        }

        UnityWebRequest scoreInfoRequest = UnityWebRequest.Get(scoreURL);

        yield return scoreInfoRequest.SendWebRequest();

        if (scoreInfoRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(scoreInfoRequest.error);
            yield break;
        }

        var rawScore = scoreInfoRequest.downloadHandler.text;

        //remove [] from string
        string result = rawScore.Remove(rawScore.Length - 1);
        result = result.Remove(0, 1);

        userScore.text = result;
    }
}