using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Делаем класс синглтон

    public UnityEvent<GameObject> OnSceneLoad;

    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver
    }

    [Header("UI")]
    public GameObject pauseScreen;
    public TMP_Text currentRecoveryDisplay;
    public TMP_Text currentMagnetDisplay;
    public TMP_Text currentPowerDisplay;
    public TMP_Text currentMoveSpeedDisplay;
    public TMP_Text currentHealthDisplay;

    public GameObject resultScreen;

    public bool isGameOver = false;

    public GameState currentState;
    public GameState previousState;

    private void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Extra" + this + "was deleted.");
            Destroy(gameObject);
        }

        DisableScreens();
    }

    

    private void Update()
    {
        switch (currentState)
        {
            case GameState.Gameplay:
                CheckForPauseAndResume();
                break;

            case GameState.Paused:
                CheckForPauseAndResume();
                break;

            case GameState.GameOver:
                if (!isGameOver)
                {
                    isGameOver = true;
                    Time.timeScale = 0f;
                    Debug.Log("Game over");
                    DisplayResults();
                }
                break;

            default:
                Debug.LogWarning("UNKNOWN STATE");
                break;
        }
    }

    public void PauseGame()
    {
        if (currentState != GameState.Paused)
        {
            previousState = currentState;
            ChangeState(GameState.Paused);
            pauseScreen.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log("Paused");
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(previousState);
            pauseScreen.SetActive(false);
            Time.timeScale = 1f;
            Debug.Log("Resume");
        }

    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
    }

    public void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void DisableScreens()
    {
        pauseScreen.SetActive(false);
        resultScreen.SetActive(false);
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);
    }

    void DisplayResults()
    {
        resultScreen.SetActive(true);
    }

}
