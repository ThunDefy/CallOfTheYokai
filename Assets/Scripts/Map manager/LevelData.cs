using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("Level data\n")]
    public int tresureRoomCount;
    public bool haveBoss = false;

    [Header("Tilemap Visuallizer Data\n")]
    public BiomeData biomeData;

    [Header("Prop Placement Data\n")]
    public List<Prop> propsToPlace;

    [Header("Agent Placer Data\n")]
    public List<EnemyScriptableObject> normalEnemys;
    public List<float> normalEnemySpawnChance;
    public List<EnemyScriptableObject> specialEnemys;
    public List<float> specialEnemySpawnChance;
    public EnemyScriptableObject bossEnemy;
}
