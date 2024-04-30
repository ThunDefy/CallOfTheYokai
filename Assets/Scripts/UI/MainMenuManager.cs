using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public PlayerData defaultPlayerData;
    private SceneController sceneController;
    private void Awake()
    {
        if (sceneController == null) sceneController = GetComponent<SceneController>();
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
}
