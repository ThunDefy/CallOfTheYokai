using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class MainMenuManager : MonoBehaviour
{
    public PlayerData defaultPlayerData;
    [Header("UI Panels")]
    public GameObject mainMenuScreen;
    public GameObject choosingYokaiScreen;
    public GameObject playerProgressionScreen;

    [Header("Chose yokai UI elements")]
    public Image yokaiImage;
    public TMP_Text yokaiName;
    public TMP_Text yokaiLore;
    public UIYokaiStatsDisplay yokaiStatsDisplay;
    public Button choseStartYokai;

    [System.Serializable]
    public struct YoakiDescription
    {
        public Button choseButton;
        public PlayerWeaponData yokaiData;
        [TextAreaAttribute]
        public string lore;
    }

    public MainMenuManager.YoakiDescription[] yoakiDescriptions;

    private PlayerData.Stats currentPlayerData;
    private SceneController sceneController;
    private int chosenStartYokai;

    private void Awake()
    {
        if (sceneController == null) sceneController = GetComponent<SceneController>();
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("PlayerStats"))
        {
            currentPlayerData = SaveAndLoadManager.LoadPlayerData();
            
        }
        else
        {
            currentPlayerData = defaultPlayerData.stats;
        }

        foreach (var item in yoakiDescriptions)
        {
            if (!currentPlayerData.availableStartingWeaponsID.Contains(item.yokaiData.yokaiID)) 
            {
                Color newColor = item.choseButton.image.color;
                newColor.a = 0.6f;
                item.choseButton.image.color = newColor;
            }
        }
    }

    public void ShowChoosingYokai(bool show)
    {
        HideAllScreens();
        if(show)
            choosingYokaiScreen.SetActive(true);
        else
            mainMenuScreen.SetActive(true);
    }

    public void ShowPlayerProgression(bool show)
    {
        HideAllScreens();
        if (show)
            playerProgressionScreen.SetActive(true);
        else
            mainMenuScreen.SetActive(true);
    }

    public void StartGameButton()
    {
        if (PlayerPrefs.HasKey("PlayerStats"))
        {
            // Сохраненные данные "PlayerStats" существуют
            sceneController.SceneChange("Game");
        }
        else
        {
            // Нет сохраненных данных "PlayerStats"
            Debug.Log("Нет сохраненных данных 'PlayerStats'.");

            PlayerData newPlayerStats = defaultPlayerData;
            SaveAndLoadManager.SavePlayerData(newPlayerStats.stats);

            Debug.Log("Новая игра начата, новый PlayerStats создан.");
            sceneController.SceneChange("Game");
        }
        
    }

    public void ShowYokaiDiscription(int yokaiIndx)
    {
        yokaiImage.sprite = yoakiDescriptions[yokaiIndx].yokaiData.icon;
        yokaiName.text = yoakiDescriptions[yokaiIndx].yokaiData.baseStats.name;
        yokaiLore.text = yoakiDescriptions[yokaiIndx].lore;
        yokaiStatsDisplay.yokaiData = yoakiDescriptions[yokaiIndx].yokaiData;
        yokaiStatsDisplay.UpdateStatFields();

        if (currentPlayerData.availableStartingWeaponsID.Contains(yokaiIndx + 1))
        {
            chosenStartYokai = yokaiIndx + 1;
            choseStartYokai.interactable = true;
            // сделать рамку ёкая зеленой
            
        }else 
            choseStartYokai.interactable = false;

    }

    public void SetStartYokai()
    {
        SaveAndLoadManager.SaveStartYokai(chosenStartYokai);  
    }

    private void HideAllScreens()
    {
        mainMenuScreen.SetActive(false);
        choosingYokaiScreen.SetActive(false);
        playerProgressionScreen.SetActive(false);
    }



}
