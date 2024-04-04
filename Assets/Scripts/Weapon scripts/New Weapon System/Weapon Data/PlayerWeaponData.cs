using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerWeaponScriptableObject", menuName = "ScriptableObjects/PlayerWeaponData")]
public class PlayerWeaponData : YokaiData
{

    public string behaviour;
    public Weapon.Stats baseStats;
    public Weapon.Stats[] linearGrowth;
    public Weapon.Stats[] randomGrowth;
    public PassiveData passiveEffectData; 


}
