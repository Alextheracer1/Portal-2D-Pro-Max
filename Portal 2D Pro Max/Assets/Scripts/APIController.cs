using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class APIController : MonoBehaviour
{
    public TextMeshProUGUI userScore;

    public TMP_InputField usernameInputLogin;
    public TMP_InputField passwordInputLogin;
    public TMP_InputField usernameInputRegister;
    public TMP_InputField passwordInputRegister;

    public TextMeshProUGUI loggedInText;
    public TextMeshProUGUI errorMessageText;

    public GameObject mainMenu;
    public GameObject loginMenu;
    public GameObject tryYourLuckMenu;
    public GameObject registerMenu;

    public Button loginButton;
    public Button logoutButton;

    private bool _loggedIn;

    private readonly string baseAPIUrl = "http://127.0.0.1:8080/api"; //TODO: Change to a proper URL later
    private string _userInformationPath;
    private readonly string baseLoginText = "Logged in as";
    private readonly string baseUUIDTemplate = "UUID: ";
    private readonly string baseUsernameTemplate = "username: ";


    private void Start()
    {
        userScore.text = "";
        errorMessageText.text = "";
        _userInformationPath = Application.persistentDataPath + "/CurrentPlayer.txt";
        CheckFile();
    }

    private void ClearErrorMessageText()
    {
        errorMessageText.text = "";
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
                Debug.Log("Logged in as " + username);
                ClearErrorMessageText();
                _loggedIn = true;
            }
            else
            {
                loginButton.gameObject.SetActive(true);
                logoutButton.gameObject.SetActive(false);
                _loggedIn = false;
            }
        }
    }


    public void OnButtonTry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3, LoadSceneMode.Single);
    }

    public void OnButtonTryYourLuck()
    {
        if (_loggedIn)
        {
            StartCoroutine(GetCurrentScore());
            mainMenu.gameObject.SetActive(false);
            tryYourLuckMenu.gameObject.SetActive(true);
        }
        else
        {
            errorMessageText.SetText("You need to be logged in to Try Your Luck!");
        }
    }

    public void OnButtonLogout()
    {
        using (StreamWriter writer = new StreamWriter(_userInformationPath))
        {
            writer.WriteLine("empty lol");
        }

        loginButton.gameObject.SetActive(true);
        logoutButton.gameObject.SetActive(false);
        _loggedIn = false;
        loggedInText.text = "";
    }

    public void OnButtonLogin()
    {
        string username = usernameInputLogin.text;
        string password = passwordInputLogin.text;

        if (username != "" && password != "")
        {
            StartCoroutine(GetUser(username, password));
        }
    }


    public void OnButtonRegister()
    {
        String username = usernameInputRegister.text;
        String password = passwordInputRegister.text;
        if (username != "" && password != "")
        {
            StartCoroutine(RegisterUser(username, password));
            StartCoroutine(GetUser(username, password));
        }
    }

    IEnumerator RegisterUser(String username, String password)
    {
        string registerUrl = baseAPIUrl + "/saveUser?username=" + username + "&password=" + password;

        UnityWebRequest checkLoginRequest = UnityWebRequest.Post(registerUrl, "");

        yield return checkLoginRequest.SendWebRequest();

        if (checkLoginRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError(checkLoginRequest.error);
            yield break;
        }
        
        usernameInputRegister.text = "";
        passwordInputRegister.text = "";
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
            registerMenu.gameObject.SetActive(false);
            ClearErrorMessageText();
            CheckFile();
        }
        else if (checkLoginRequest.responseCode == 400)
        {
            Debug.Log("Invalid Credentials");
        }
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
            userScore.text = "0";
            yield break;
        }

        var rawScore = scoreInfoRequest.downloadHandler.text;

        //remove [] from string
        string result = rawScore.Remove(rawScore.Length - 1);
        result = result.Remove(0, 1);

        try
        {
            Int32.Parse(result);
            userScore.text = result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            userScore.text = "0";
        }
    }
}