using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerStats : MonoBehaviour
{
    [HideInInspector]
    public Health healthData;
    [HideInInspector]
    public Agent playerAgentData;
    [HideInInspector]
    public WeaponParent weaponParent;


    public PlayerData playerData;
    public PlayerData.Stats baseStats;
    [SerializeField] 
    public PlayerData.Stats actualStats;
    float currentHealth;


    PlayerCollector collector;

    //public List<GameObject> spawnedWeapons;

    public int weaponIndex;
    public List<PlayerWeaponData> allWeapons;

    //public GameObject startingWeapon;
    

    [Header("Player stats")]
    public PlayerData player;
    public int availableSlotsCount;

    #region Current stats
    public float CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            if (currentHealth != value)
            {
                currentHealth = value;
            }
        }
    }

    public float CurrentMaxHealth
    {
        get { return actualStats.maxHealth; }
        set
        {
            if (actualStats.maxHealth != value)
            {
                actualStats.maxHealth = value;
                healthData.maxHealth = actualStats.maxHealth;
            }
        }
    }
    public float CurrentMoveSpeed
    {
        get { return actualStats.moveSpeed; }
        set
        {
            if (actualStats.moveSpeed != value)
            {
                actualStats.moveSpeed = value;
                playerAgentData.moveSpeed = actualStats.moveSpeed;

            }
        }
    }
    #endregion


    // ќпыт и повышение уровн€
    [Header("Experience and Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    [Header("Other stats")]
    public float invincibilityDuration;
    float invincibilityTimer;

    bool isInvincible;
    [HideInInspector]
    public bool canTakeDamage = true;

    [Header("Visuals")]
    public ParticleSystem blockedEffect;

    PlayerInventory playerInventory;

    [Header("UI")]
    public Image expBar;
    public TMP_Text levelText;


    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    public List<LevelRange> levelRanges;

    void Awake()
    {
        healthData = GetComponent<Health>();
        playerAgentData = GetComponent<Agent>();
        weaponParent = GetComponentInChildren<WeaponParent>();
        //inventory = GetComponent<InventoryManager>();
        collector = GetComponentInChildren<PlayerCollector>();

        playerData.stats = SaveAndLoadManager.LoadPlayerData(); // «агружаем сохраненную статистику  (если нет сохр. то одни нули)
        playerInventory = GetComponent<PlayerInventory>();
        baseStats = actualStats = playerData.stats;
        currentHealth = actualStats.maxHealth;
        availableSlotsCount = baseStats.availableSlots;

        healthData.maxHealth = actualStats.maxHealth;
        healthData.currentHealth = actualStats.maxHealth;
        playerAgentData.moveSpeed = actualStats.moveSpeed;

        collector.SetRadius(actualStats.magnet);

    }

    private void Start()
    {
        //playerInventory.AddYokai(playerData.startingWeapon1);
        //playerInventory.AddYokai(playerData.startingWeapon2);
        //playerInventory.AddYokai(playerData.startingWeapon3);
        //playerInventory.AddYokai(playerData.startingWeapon4);

        weaponIndex = allWeapons.FindIndex(weapon => weapon.yokaiID == playerData.stats.startingYokaiID);
        if(weaponIndex != -1)
            playerInventory.AddYokai(allWeapons[weaponIndex]);

        //experienceCap += levelRanges[0].experienceCapIncrease;

        UpdateExpBar();
        UpdateLevelText();
    }

    private void Update()
    {
        if(invincibilityTimer > 0)
            invincibilityTimer -= Time.deltaTime;
        else if(isInvincible) 
            isInvincible = false;
        Recover();
    }

   
    public void IncreaseExperience(int amount, bool isRareSoul = false)
    {
        experience += (int)Mathf.Round(amount * actualStats.growth);
        if (isRareSoul)
        {
            actualStats.specialSouls += amount;
        }
        else
        {
            actualStats.commonSouls += amount;
        }
        
        LevelUpChecker();
        UpdateExpBar();
    }

    public void LevelUpChecker(bool force = false)
    {
        if(force || experience >= experienceCap)
        {
            
            level++;
            experience -= experienceCap;
            int experienceCapIncrease = 0;
            foreach (LevelRange range in levelRanges)
            {
                if(level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                    break;
                }    
            }
            experienceCap += experienceCapIncrease;

            UpdateLevelText();
            GameManager.instance.StartLevelUp();

        }
    }

    void UpdateExpBar()
    {
        expBar.fillAmount = (float)experience / experienceCap;
    }

    void UpdateLevelText()
    {
        levelText.text = level.ToString();
    }

    public void RestoreHealth(float amount)
    {
        if(healthData.currentHealth < healthData.maxHealth)
        {
            healthData.currentHealth += amount;

            if (healthData.currentHealth > healthData.maxHealth)
            {
                healthData.currentHealth = healthData.maxHealth;
            }

            healthData.UpdateHealthBar();
        }
        CurrentHealth = healthData.currentHealth;
    }

    void Recover()
    {
        if(healthData.currentHealth < healthData.maxHealth)
        {
            healthData.currentHealth += actualStats.recovery * Time.deltaTime;
            if (healthData.currentHealth > healthData.maxHealth)
                healthData.currentHealth = healthData.maxHealth;
        }
        CurrentHealth = healthData.currentHealth;
        healthData.UpdateHealthBar();

    }

    public void PlayerDie()
    {
        if(actualStats.revival > 0)
        {
            actualStats.revival -= 1;
            
            healthData.currentHealth = actualStats.maxHealth * 0.5f;
        }
        else if (!GameManager.instance.isGameOver)
        {
            healthData.isDead = true;
            GameManager.instance.AssignLevelReachedUI(level);
            GameManager.instance.AssignChosenWeaponsUI(playerInventory.weaponSlots);
            GameManager.instance.GameOver();
            
        }
    }

    public void RecalculateStats() 
    {
        actualStats = baseStats;
        foreach (PlayerInventory.Slot s in playerInventory.passiveSlots)
        {
            Passive p = s.yokai as Passive;
            if (p)
            {
                actualStats += p.GetBoosts();
            }
        }
        playerAgentData.moveSpeed = actualStats.moveSpeed;
        healthData.maxHealth = actualStats.maxHealth;

    }

    internal void TakeDamage(float dmg, GameObject gameObject, Vector3 position)
    {
        if (!isInvincible && canTakeDamage)
        {
            dmg -= actualStats.armor;
            if(dmg > 0)
            {
                healthData.GetHit(dmg, gameObject, position);
            }
            else
            {
                if (blockedEffect) Destroy(Instantiate(blockedEffect, transform.position, Quaternion.identity), 5f);
            }
            invincibilityTimer = invincibilityDuration;
            isInvincible = true;
        }
        
    }
}
