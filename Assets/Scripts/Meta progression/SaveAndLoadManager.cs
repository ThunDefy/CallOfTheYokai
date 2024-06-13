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

    public static void SaveRunProgress(int specialSoulsCount=0, int commonSoulsCount=0)
    {
        PlayerData.Stats playerStats = LoadPlayerData();
        playerStats.specialSouls += specialSoulsCount;
        playerStats.commonSouls += commonSoulsCount;
        SavePlayerData(playerStats);
    }

    public static void SaveStartYokai(int yokaiID)
    {
        PlayerData.Stats playerStats = LoadPlayerData();
        playerStats.startingYokaiID = yokaiID;
        SavePlayerData(playerStats);
    }

    public static void SavePermanentPassiveBonus(PlayerData.Stats bonus, int yokaiID)
    {
        PlayerData.Stats playerStats = LoadPlayerData();
        playerStats += bonus;
        playerStats.availableStartingWeaponsID.Add(yokaiID);
        SavePlayerData(playerStats);
    }
}
