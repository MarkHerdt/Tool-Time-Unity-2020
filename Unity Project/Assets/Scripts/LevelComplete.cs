using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{

    bool isLevelWon;
    bool levelFinished;

    public Text textLevelEnd;

    public GameObject buttonRestart;
    public GameObject buttonQuit;

    private void OnEnable()
    {
        EventSystemController.self.onVictoryIsInDanger += IsLevelLost;
        EventSystemController.self.onGlobalTimerEnd += GlobalTimerEnd;

        isLevelWon = true;
        levelFinished = false;
    }

    void Start()
    {


    }

    private void Update()
    {
        if (levelFinished)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                RestartLevel();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitGame();
            }
        }
    }

    void IsLevelLost(bool levelLost)
    {
        if (levelLost)
        {
            isLevelWon = false;
        }
        else
        {
            isLevelWon = true;
        }
    }

    void GlobalTimerEnd()
    {
        Debug.Log("1");
        levelFinished = true;
        if (isLevelWon)
        {
            Debug.Log("2");
            textLevelEnd.text = "Das Raumschiff ist gelandet!";
        }
        else
        {
            Debug.Log("3");
            textLevelEnd.text = "Das Raumschiff ist abgestürzt!";
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Spiel beendet!");
        Application.Quit();
    }
}