using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "ScriptableObjects/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    public enum EnemyType
    {
        Normal,
        Boss,
        Guardian,

    }

    public GameObject enemyPrefab;
    public EnemyType enemyType;
}
