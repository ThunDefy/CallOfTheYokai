using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerDataScriptableObject", menuName = "ScriptableObjects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField]
    public string playerName;
    [SerializeField]
    public PlayerWeaponData startingWeapon;

    [System.Serializable]
    public struct Stats
    {
        public float maxHealth, recovery, moveSpeed;
        public float power, speed, magnet;

        public Stats(float maxHealth = 10, float recovery = 0.5f, float moveSpeed = 1f, float power = 1f, float speed = 3f, float magnet = 1.6f)
        {
            this.maxHealth = maxHealth;
            this.recovery = recovery;
            this.moveSpeed = moveSpeed;
            this.power = power;
            this.speed = speed;
            this.magnet = magnet;
              
        }

        public static Stats operator +(Stats s1, Stats s2)
        {
            s1.maxHealth += s2.maxHealth;
            s1.recovery += s2.recovery;
            s1.moveSpeed += s2.moveSpeed;
            s1.power += s2.power;
            s1.speed += s2.speed;
            s1.magnet += s2.magnet;
            return s1;
        }
    }

    public Stats stats = new Stats();
}
