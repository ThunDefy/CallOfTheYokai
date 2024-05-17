using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsCollector : MonoBehaviour
{

    public static StatisticsCollector instance; // Делаем класс синглтон

    
    [System.Serializable]
    private class GameSessionStatistics
    {
        public int totalEnemyKillCount = 0;
        public int levelReached = 0;
        public int totalCommonSoulsCount = 0;
        public int totalRareSoulsCount = 0;
    }
    [System.Serializable]
    private class CurrentLevelStatistics
    {
        public int currentEnemyKillCount = 0;
        public int currentCommonSoulsCount = 0;
        public int currentRareSoulsCount = 0;
    }

    private GameSessionStatistics sessionStats;
    private CurrentLevelStatistics levelStats;

    [Header("Game Session Result Screen Displays")]
    public TMP_Text levelReachedDisplay;
    public TMP_Text enemyKillCountDisplay;
    public TMP_Text commonSoulsCountDisplay;
    public TMP_Text rareSoulsCountDisplay;

    [Header("Level Result Screen Displays")]
    public TMP_Text currentEnemyKillCountDisplay;
    public TMP_Text currentCommonSoulsCountDisplay;
    public TMP_Text currentRareSoulsCountDisplay;


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

        sessionStats = new GameSessionStatistics();
        levelStats = new CurrentLevelStatistics();

    }

    public void ShowLevelResult()
    {
        currentEnemyKillCountDisplay.text = levelStats.currentEnemyKillCount.ToString();
        currentCommonSoulsCountDisplay.text = levelStats.currentCommonSoulsCount.ToString();
        currentRareSoulsCountDisplay.text = levelStats.currentRareSoulsCount.ToString();
        UpdateSessionStatistics();
    }

    public void ShowGameResult()
    {
        UpdateSessionStatistics();
        levelReachedDisplay.text = sessionStats.levelReached.ToString();
        enemyKillCountDisplay.text = sessionStats.totalEnemyKillCount.ToString();
        commonSoulsCountDisplay.text = sessionStats.totalCommonSoulsCount.ToString();
        rareSoulsCountDisplay.text = sessionStats.totalRareSoulsCount.ToString();  
    }

    public void IncreaseKillCount()
    {
        levelStats.currentEnemyKillCount += 1;
    }

    public void IncreaseCommonSoulsCount()
    {
        levelStats.currentCommonSoulsCount += 1;
    }

    public void IncreaseRareSoulsCount()
    {
        levelStats.currentRareSoulsCount += 1;
    }

    public void UpdatelevelReached(int newLevel)
    {
        sessionStats.levelReached = newLevel;
    }

    private void UpdateSessionStatistics()
    {
        sessionStats.totalEnemyKillCount += levelStats.currentEnemyKillCount;
        sessionStats.totalCommonSoulsCount += levelStats.currentCommonSoulsCount;
        sessionStats.totalRareSoulsCount += levelStats.currentRareSoulsCount;
        levelStats = new CurrentLevelStatistics();
    }






}
