using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "ScriptableObjects/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    [System.Serializable]
    public struct EnemyWeaponStats
    {
        public GameObject prefab;
        public string target;
        public float damage;
        public float speed;
        public float cooldownDuration;
        //public float knockback;
    }

    // Базовые характеристики оружия
    [Header("Base Stats")]

    public GameObject prefab;
    public string target;
    public float damage;
    public float speed;
    public float cooldownDuration;
    //public float knockback;

    [SerializeField]
    public WeaponScriptableObject.EnemyWeaponStats[] weaponStats;

}
