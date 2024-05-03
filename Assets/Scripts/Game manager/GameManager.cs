using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static PlayerInventory;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Делаем класс синглтон

    public UnityEvent<GameObject> OnSceneLoad;

    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver,
        SuccessfulEnd,
        LevelUp,
        ChangeWeapon,
        RisingWeapon,
        ShowNewWeapon
    }

    [Header("Damage Text Settings")]
    public Canvas damageTextCanvas;
    public float textFontSize = 20;
    public TMP_FontAsset textFont;
    public Camera referenceCamera;

    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject resultScreen;
    public GameObject getPassiveBonusScreen;
    public GameObject levelUpScreen;
    public GameObject changeWeaponScreen;
    public GameObject risingWeaponScreen;
    public GameObject newWeaponScreen;

    public TMP_Text timerDisplay;

    [Header("Result Screen Displays")]
    public TMP_Text levelReachedDisplay;
    public List<Image> chosenWeaponsUI = new List<Image>(2);
    public TMP_Text gameTimeDisplay;

    float timer;

    public bool isGameOver = false;
    public bool isSuccessfulEnd = false;
    public bool choosingUpgrade;
    public bool changingWeapon = false;
    public bool risingWeapon = false;
    public bool newWeapon = false;

    public GameState currentState;
    public GameState previousState;

    public GameObject player;
    PlayerStats pd;

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
        pd = player.GetComponent<PlayerStats>();
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
                    //Debug.Log("Game over"); 
                    DisplayResults();
                }
                break;

            case GameState.SuccessfulEnd:
                if (!isSuccessfulEnd)
                {
                    isSuccessfulEnd = true;
                    Time.timeScale = 0f;
                    DisplayPermanentPassiveBoost();
                }
                break;

            case GameState.LevelUp:
                if (!choosingUpgrade)
                {
                    choosingUpgrade = true;
                    Time.timeScale = 0f;
                    //Debug.Log("choosing Upgrade");
                    levelUpScreen.SetActive(true);
                }
                break;
            case GameState.ChangeWeapon:
                if (!changingWeapon)
                {
                    changingWeapon = true;
                    Time.timeScale = 0f;
                    //Debug.Log("changing Weapon");
                    changeWeaponScreen.SetActive(true);

                }
                break;
            case GameState.RisingWeapon:
                if (!changingWeapon)
                {
                    risingWeapon = true;
                    Time.timeScale = 0f;
                    //Debug.Log("rising Up Weapon");
                    risingWeaponScreen.SetActive(true);

                }
                break;
            case GameState.ShowNewWeapon:
                if (!changingWeapon)
                {
                    newWeapon = true;
                    Time.timeScale = 0f;
                    newWeaponScreen.SetActive(true);

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
            player.SendMessage("ShowWeaponsStatInPauseMenu");
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
        changeWeaponScreen.SetActive(false);
        risingWeaponScreen.SetActive(false);
        getPassiveBonusScreen.SetActive(false);
    }

    public void GameOver()
    {
        UpdateTimerDisplay();
        gameTimeDisplay.text = timerDisplay.text;
        ChangeState(GameState.GameOver);
        SavePlayerProgress();
    }

    void DisplayResults()
    {
        resultScreen.SetActive(true);
    }

    void DisplayPermanentPassiveBoost()
    {
        getPassiveBonusScreen.SetActive(true);
    }

    public void AssignLevelReachedUI(int levelReachedData)
    {
        levelReachedDisplay.text = levelReachedData.ToString();
    }

    public void AssignChosenWeaponsUI(List<PlayerInventory.Slot> chosenWeaponsData)
    {

        if (chosenWeaponsData.Count != chosenWeaponsUI.Count) return;

        for (int i = 0; i < chosenWeaponsUI.Count; i++)
        {
            if (chosenWeaponsData[i].image.sprite != null)
            {
                chosenWeaponsUI[i].enabled = true;
                chosenWeaponsUI[i].sprite = chosenWeaponsData[i].image.sprite;
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
    public void StartShowNewWeapon()
    {
        ChangeState(GameState.ShowNewWeapon);
    }

    public void EndShowNewWeapon()
    {
        newWeapon = false;
        Time.timeScale = 1f;
        newWeaponScreen.SetActive(false);
        ChangeState(GameState.Gameplay);

    }

    public void StartChangingWeapon()
    {
        ChangeState(GameState.ChangeWeapon);
    }

    public void EndChangingWeapon()
    {
        changingWeapon = false;
        Time.timeScale = 1f;
        changeWeaponScreen.SetActive(false);
        ChangeState(GameState.Gameplay);
    }

    public void StartRisingWeapon()
    {
        ChangeState(GameState.RisingWeapon);
    }

    public void EndRisingWeapon()
    {
        risingWeapon = false;
        Time.timeScale = 1f;
        risingWeaponScreen.SetActive(false);
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
        textObj.transform.SetSiblingIndex(0);

        // Анимация текста
        Vector3 initialTargetPosition = target.position;
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        float time = 0;
        float yOffset = 0;
        while(time < duration-0.2f)
        {
            yield return wait;
            time += Time.deltaTime;

            textMP.color = new Color(textMP.color.r,textMP.color.g,textMP.color.b,1- time / duration);

            yOffset += speed * Time.deltaTime;
            rect.position = referenceCamera.WorldToScreenPoint(initialTargetPosition + new Vector3(0, yOffset));

        }


    }

    
    public void StartChosePermanentPassiveBoost()
    {
        player.SendMessage("SetPossiblePermanentPassiveBonuses");
        ChangeState(GameState.SuccessfulEnd);
    }

    public void EndChosePermanentPassiveBoost()
    {
        isSuccessfulEnd = false;
        getPassiveBonusScreen.SetActive(false);
        GameOver();
    }

    public void SavePlayerProgress()
    {
        SaveAndLoadManager.SaveRunProgress(pd.actualStats.specialSouls, pd.actualStats.commonSouls);
    }

    public void LoadPlayerProgress()
    {
        pd.playerData.stats = SaveAndLoadManager.LoadPlayerData();
        print("now specialSouls = " + pd.playerData.stats.specialSouls);
    }
}
