using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerWeaponScriptableObject", menuName = "ScriptableObjects/PlayerWeaponData")]
public class PlayerWeaponData : YokaiData
{

    public string behaviour;
    public GameObject yokaiPrefab;
    public Weapon.Stats baseStats;
    public Weapon.Stats[] linearGrowth;
    public Weapon.Stats[] randomGrowth;
    public PassiveData passiveEffectData;

    public Weapon.Stats GetLevelData(int indx)
    {
        //if (level - 2 < linearGrowth.Length)
        //{
        //    return linearGrowth[level - 2];
        //}
        //if (randomGrowth.Length > 0)
        //{
        //    return randomGrowth[Random.Range(0, randomGrowth.Length)];
        //}
        //return new Weapon.Stats();
        return randomGrowth[indx];
    }

    public int GetRandomLevelData()
    {
        return Random.Range(0, randomGrowth.Length);
    }
}
