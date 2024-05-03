using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public List<LevelData> levelsData;
    private int currentLevelIndex = 0;
    public int CurrentLevelIndex { get => currentLevelIndex; }

    RoomFirstMapGenerator roomFirstMapGenerator;
    TilemapVisualizer tilemapVisualizer;
    AgentPlacer agentPlacer;
    PropPlacementManager propPlacementManager;
    
    private void Awake()
    {
        roomFirstMapGenerator = FindObjectOfType<RoomFirstMapGenerator>();
        tilemapVisualizer = FindObjectOfType<TilemapVisualizer>();
        agentPlacer = FindObjectOfType<AgentPlacer>();
        propPlacementManager = FindObjectOfType<PropPlacementManager>();
    }

    public LevelData GetCurrentLevelData()
    {
        if (currentLevelIndex < levelsData.Count)
        {
            return levelsData[currentLevelIndex];
        }

        return null;
    }

    public void SwitchToNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex >= levelsData.Count)
        {
            // ������� ���������� ������!
            GameManager.instance.StartChosePermanentPassiveBoost();
        }
        else if (roomFirstMapGenerator != null)
        {
            print("currentLevelIndex "+currentLevelIndex);
            tilemapVisualizer.UpdateLevelData();
            agentPlacer.UpdateLevelData();
            propPlacementManager.UpdateLevelData();
            roomFirstMapGenerator.UpdateLevelData();
            roomFirstMapGenerator.GenerateMap();
        }
    }
}
