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
    public float multiplier = 5;


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
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(1))
        {
            _currentTime += multiplier;
        }

        if (countScore)
        {
            _currentTime += Time.deltaTime;
        }

        _score = Mathf.RoundToInt(_currentTime * multiplier);
        //Debug.Log("Current Score: " + _score);
    }


    IEnumerator SaveScore()
    {
        string saveScoreURL = "localhost:8080/api/saveScore/";

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