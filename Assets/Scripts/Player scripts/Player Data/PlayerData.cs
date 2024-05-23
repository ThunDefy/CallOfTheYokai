using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerDataScriptableObject", menuName = "ScriptableObjects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField]
    public string playerName;
    [SerializeField]
    public PlayerWeaponData startingWeapon1;
    [SerializeField]
    public PlayerWeaponData startingWeapon2;
    [SerializeField]
    public PlayerWeaponData startingWeapon3;
    [SerializeField]
    public PlayerWeaponData startingWeapon4;

    [System.Serializable]
    public struct Stats
    {

        public float maxHealth, recovery, armor;
        [Range(-1, 10)] public float moveSpeed, power, area;
        [Range(-1, 5)] public float speed;
        [Range(0, 1)] public float cooldown;
        [Range(0, 1)] public float luck;
        [Min(-1)] public float growth;
        public float magnet;
        public int revival;
        public int commonSouls, specialSouls,  availableSlots, startingYokaiID;
        //public List<int> startingWeaponsID;
        public List<int> availableStartingWeaponsID;

        public static Stats operator +(Stats s1, Stats s2)
        {
            s1.maxHealth += s2.maxHealth;
            s1.recovery += s2.recovery;
            s1.armor += s2.armor;
            s1.moveSpeed += s2.moveSpeed;
            s1.power += s2.power;
            s1.speed += s2.speed;
            s1.magnet += s2.magnet;
            s1.cooldown += s2.cooldown;
            s1.luck += s2.luck;
            s1.growth += s2.growth;
            s1.revival += s2.revival;
            s1.area += s2.area;
            s1.commonSouls += s2.commonSouls;
            s1.specialSouls += s2.specialSouls;
            s1.availableSlots += s2.availableSlots;

            return s1;
        }
    }

    public Stats stats = new Stats
    {
        maxHealth = 100, moveSpeed = 1, recovery = 1, power = 1, 
        area = 1, speed = 1,  cooldown = 1,
        luck = 0,  growth = 1, 
        specialSouls = 0, commonSouls = 0,
        availableSlots = 2,
        startingYokaiID = 0
    };
}
