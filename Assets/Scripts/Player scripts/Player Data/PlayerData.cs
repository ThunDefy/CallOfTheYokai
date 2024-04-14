using System.Collections;
using System.Collections.Generic;
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
        //public float maxHealth, recovery, moveSpeed;
        //public float power, speed, magnet;

        public float maxHealth, recovery, armor;
        [Range(-1, 10)] public float moveSpeed, power, area;
        [Range(-1, 5)] public float speed, duration;
        [Range(-1, 10)] public int amount;
        [Range(-1, 1)] public float cooldown;
        [Min(-1)] public float luck, growth, greed, curse;
        public float magnet;
        public int revival;

        //public Stats(float maxHealth = 10, float recovery = 0.5f, float moveSpeed = 1f, float power = 1f, float speed = 3f, float magnet = 1.6f)
        //{
        //    this.maxHealth = maxHealth;
        //    this.recovery = recovery;
        //    this.moveSpeed = moveSpeed;
        //    this.power = power;
        //    this.speed = speed;
        //    this.magnet = magnet;
              
        //}

        public static Stats operator +(Stats s1, Stats s2)
        {
            s1.maxHealth += s2.maxHealth;
            s1.recovery += s2.recovery;
            s1.armor += s2.armor;
            s1.duration += s2.duration;
            s1.amount += s2.amount;
            s1.moveSpeed += s2.moveSpeed;
            s1.power += s2.power;
            s1.speed += s2.speed;
            s1.magnet += s2.magnet;
            s1.cooldown += s2.cooldown;
            s1.luck += s2.luck;
            s1.growth += s2.growth;
            s1.greed += s2.greed;
            s1.revival += s2.revival;
            s1.curse += s2.curse;
            s1.area += s2.area;
            return s1;
        }
    }

    public Stats stats = new Stats
    {
        maxHealth = 100, moveSpeed = 1, recovery = 1, power = 1, amount = 0,
        area = 1, speed = 1, duration = 1, cooldown = 1,
        luck = 1, greed = 1, growth = 1, curse = 1
    };
}
