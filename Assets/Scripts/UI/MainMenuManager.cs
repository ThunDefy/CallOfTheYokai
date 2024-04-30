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
            // ����������� ������ "PlayerStats" ����������
            sceneController.SceneChange("Game");
        }
        else
        {
            // ��� ����������� ������ "PlayerStats"
            Debug.Log("��� ����������� ������ 'PlayerStats'.");

            PlayerData newPlayerStats = defaultPlayerData;
            SaveAndLoadManager.SavePlayerData(newPlayerStats.stats);

            Debug.Log("����� ���� ������, ����� PlayerStats ������.");
            sceneController.SceneChange("Game");
        }
        
    }
}
