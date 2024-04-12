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
    public Weapon.Stats[] risingGrowth;
    public Weapon.Stats[] randomGrowth;
    public PassiveData passiveEffectData;

    public Weapon.Stats GetRisingLevelData(int level)
    {
        if (level - 1 < risingGrowth.Length)
        {
            return risingGrowth[level - 1];
        }
        else
        {
            Debug.LogWarning("ƒостигнут максимальный уровень возвышений");
            // ƒостигнут максимальный уровень возвышени€
            // ѕросто дать игроку уровень, как то..
            return new Weapon.Stats();
        }

    }
    public Weapon.Stats GetLevelData(int indx)
    {
        return randomGrowth[indx];
    }

    public int GetRandomLevelData()
    {
        return Random.Range(0, randomGrowth.Length);
    }
}
