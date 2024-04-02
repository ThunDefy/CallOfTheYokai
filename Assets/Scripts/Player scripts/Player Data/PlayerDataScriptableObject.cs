using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerDataScriptableObject", menuName = "ScriptableObjects/PlayerData")]
public class PlayerDataScriptableObject : ScriptableObject
{
    // Базовые статистики врагов
    public float moveSpeed;
    public float maxHealth;
    public float power;
    public float recovery;
    public float magnet;
}
