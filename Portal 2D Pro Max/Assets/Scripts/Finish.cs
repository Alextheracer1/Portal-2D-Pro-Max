using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{

   
    private void OnTriggerEnter2D(Collider2D col)
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        
        if (currentScene == 3)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex * 0, LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
        }
        
        
        
    }
}
