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
        GameOver,
        LevelUp
    }

    [Header("Damage Text Settings")]
    public Canvas damageTextCanvas;
    public float textFontSize = 20;
    public TMP_FontAsset textFont;
    public Camera referenceCamera;

    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject resultScreen;
    public GameObject levelUpScreen;

    [Header("Pause Displays")]
    public TMP_Text currentRecoveryDisplay;
    public TMP_Text currentMagnetDisplay;
    public TMP_Text currentPowerDisplay;
    public TMP_Text currentMoveSpeedDisplay;
    public TMP_Text currentHealthDisplay;
    public TMP_Text currentProjectileSpeedDisplay;
    public TMP_Text timerDisplay;

    [Header("Result Screen Displays")]
    public TMP_Text levelReachedDisplay;
    public List<Image> chosenWeaponsUI = new List<Image>(2);
    public TMP_Text gameTimeDisplay;

    float timer;

    public bool isGameOver = false;
    public bool choosingUpgrade;

    public GameState currentState;
    public GameState previousState;

    public GameObject player;

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
                UpdateTimer();
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

            case GameState.LevelUp:
                if (!choosingUpgrade)
                {
                    choosingUpgrade = true;
                    Time.timeScale = 0f;
                    Debug.Log("choosing Upgrade");
                    levelUpScreen.SetActive(true);
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
                UpdateTimerDisplay();
                PauseGame();
            }
        }
    }

    private void DisableScreens()
    {
        pauseScreen.SetActive(false);
        resultScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }

    public void GameOver()
    {
        UpdateTimerDisplay();
        gameTimeDisplay.text = timerDisplay.text;
        ChangeState(GameState.GameOver);
    }

    void DisplayResults()
    {
        resultScreen.SetActive(true);
    }

    public void AssignLevelReachedUI(int levelReachedData)
    {
        levelReachedDisplay.text = levelReachedData.ToString();
    }

    public void AssignChosenWeaponsUI(List<Image> chosenWeaponsData)
    {
        if (chosenWeaponsData.Count != chosenWeaponsUI.Count) return;

        for (int i = 0; i < chosenWeaponsUI.Count; i++)
        {
            if (chosenWeaponsData[i].sprite)
            {
                chosenWeaponsUI[i].enabled = true;
                chosenWeaponsUI[i].sprite = chosenWeaponsData[i].sprite;
            }
            else
            {
                chosenWeaponsUI[i].enabled = false;
            }
        }
    }

    void UpdateTimer()
    {
        timer += Time.deltaTime;
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);

        timerDisplay.text = string.Format("{0:00}:{1:00}",minutes,seconds);
    }

    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        player.SendMessage("ApplyUpgradeOptions");
    }

    public void EndLevelUp()
    {
        choosingUpgrade = false;
        Time.timeScale = 1f;
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);

    }

    public static void GenerateFloatingText(string text, Transform target, float duration =1f, float speed = 1f)
    {
        if (!instance.damageTextCanvas) return;

        if (!instance.referenceCamera) instance.referenceCamera = Camera.main;

        instance.StartCoroutine(instance.GenerateFloatingTextCoroutine(
            text, target, duration, speed));
    }

    IEnumerator GenerateFloatingTextCoroutine(string text, Transform target, float duration=1f, float speed=50f)
    {
        GameObject textObj = new GameObject("Damage Floating Text");
        RectTransform rect = textObj.AddComponent<RectTransform>();
        TextMeshProUGUI textMP = textObj.AddComponent<TextMeshProUGUI>();

        textMP.text = text; 
        textMP.horizontalAlignment = HorizontalAlignmentOptions.Center;
        textMP.verticalAlignment = VerticalAlignmentOptions.Middle;
        textMP.fontSize = textFontSize;

        if(textFont) textMP.font = textFont;
        rect.position = referenceCamera.WorldToScreenPoint(target.position);

        Destroy(textObj,duration);

        textObj.transform.SetParent(instance.damageTextCanvas.transform);

        // Анимация текста
        Vector3 initialTargetPosition = target.position;
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        float time = 0;
        float yOffset = 0;
        while(time < duration-0.1f)
        {
            yield return wait;
            time += Time.deltaTime;

            textMP.color = new Color(textMP.color.r,textMP.color.g,textMP.color.b,1- time / duration);

            yOffset += speed * Time.deltaTime;
            rect.position = referenceCamera.WorldToScreenPoint(initialTargetPosition + new Vector3(0, yOffset));

        }

    }
}
