using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveAndLoadManager 
{
    public static void SavePlayerData(PlayerData.Stats playerStats)
    {
        string json = JsonUtility.ToJson(playerStats);
        PlayerPrefs.SetString("PlayerStats", json);

    }

    public static PlayerData.Stats LoadPlayerData()
    {
        PlayerData.Stats loadedStats = new PlayerData.Stats();
        if (PlayerPrefs.HasKey("PlayerStats"))
        {
            string json = PlayerPrefs.GetString("PlayerStats");
            try
            {
                loadedStats = JsonUtility.FromJson<PlayerData.Stats>(json);
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading player data: " + e.Message);
            }
        }
        return loadedStats;
    }

    public static void SaveRunProgress(int specialSoulsCount)
    {
        PlayerData.Stats playerStats = LoadPlayerData();
        playerStats.specialSouls = specialSoulsCount;
        SavePlayerData(playerStats);
    }
}
