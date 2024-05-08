using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("Random Walk Generation Data\n")]
    public SimpleRandomWalkData randomWalkParameters;
    public int minRoomWidth = 4, minRoomHeight = 4;
    public int dungeonWidth = 20, dungeonHeight = 20;
    [Range(0, 10)]
    public int offset = 1;

    [Header("Tilemap Visuallizer Data\n")]
    public BiomeData biomeData;

    [Header("Level data\n")]
    public int tresureRoomCount;
    public bool haveBoss = false;

    [Header("Prop Placement Data\n")]
    public List<Prop> propsToPlace;

    [Header("Agent Placer Data\n")]
    public int minRoomEnemiesCount = 1;
    public int maxRoomEnemiesCount = 10;
    public int guardiansCount = 2;

    public List<EnemyScriptableObject> normalEnemys;
    public List<float> normalEnemySpawnChance;
    public List<EnemyScriptableObject> specialEnemys;
    public List<float> specialEnemySpawnChance;
    public EnemyScriptableObject bossEnemy;
}
