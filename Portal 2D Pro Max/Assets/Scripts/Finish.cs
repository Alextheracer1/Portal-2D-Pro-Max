using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    private float _currentTime;
    public bool countScore;
    private int _score;
    public float multiplier;
    public int baseScore = 10000;


    private void StopScoreCalculation()
    {
        countScore = false;
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        if (currentScene == 3)
        {
            StopScoreCalculation();
            StartCoroutine(SaveScore());
        }
        else if (currentScene == 2)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex * 0);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex * 0, LoadSceneMode.Single);
        }
        
        if (countScore)
        {
            _currentTime += Time.smoothDeltaTime;
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                _currentTime += 5;
            }
        }


        _score = Mathf.RoundToInt(_currentTime * multiplier);
        _score = baseScore - _score;

        if (_score < 0)
        {
            _score = 0;
        }

        //Debug.Log("Current Score: " + _score);
    }


    IEnumerator SaveScore()
    {
        string saveScoreURL = "https://portal-2d-pro-max-backend.herokuapp.com/api/saveScore/";

        string[] readText = File.ReadAllLines(Application.persistentDataPath + "/CurrentPlayer.txt");
        for (int i = 0; i < readText.Length; i++)
        {
            if (readText[i].Contains("UUID: "))
            {
                string uuid = readText[i].Replace("UUID: ", "");
                saveScoreURL = saveScoreURL + uuid + "/" + _score;
                Debug.Log(saveScoreURL);

                UnityWebRequest saveScoreRequest = UnityWebRequest.Post(saveScoreURL, "");

                yield return saveScoreRequest.SendWebRequest();

                Debug.Log("Result: " + saveScoreRequest.result);
                Debug.Log("ResponseCode: " + saveScoreRequest.responseCode);

                if (saveScoreRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.LogError(saveScoreRequest.error);
                    yield break;
                }

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex * 0, LoadSceneMode.Single);
            }
        }
    }
}