using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "ScriptableObjects/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    // Базовые характеристики оружия
    [Header("Weapon Stats")]

    public GameObject prefab;
    public string target;
    public float damage;
    public float speed;
    public float cooldownDuration;
    public int pierce;
    public int Level;
    public Sprite Icon;
}
