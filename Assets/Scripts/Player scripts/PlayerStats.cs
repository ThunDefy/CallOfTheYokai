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
    [SerializeField] PlayerData.Stats actualStats;
    float currentHealth;


    PlayerCollector collector;

    //public List<GameObject> spawnedWeapons;

    public int weaponIndex;

    //public GameObject startingWeapon;
    

    [Header("Player stats")]
    public PlayerData player;

    


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
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = "Max Health: " + actualStats.maxHealth;
                }

            }
        }
    }

    public float CurrentRecovery
    {
        get { return actualStats.recovery; }
        set
        {
            if (actualStats.recovery != value)
            {
                actualStats.recovery = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + actualStats.recovery;
                }
            }
        }
    }

    public float CurrentMagnet
    {
        get { return actualStats.magnet; }
        set
        {
            if (actualStats.magnet != value)
            {
                actualStats.magnet = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMagnetDisplay.text = "Magnet: " + actualStats.magnet;
                }
            }
        }
    }

    public float CurrentPower
    {
        get { return actualStats.power; }
        set
        {
            if (actualStats.power != value)
            {
                actualStats.power = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentPowerDisplay.text = "Power: " + actualStats.power;
                }
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
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + actualStats.moveSpeed;
                }
            }
        }
    }
    #endregion


    // Опыт и повышение уровня
    [Header("Experience and Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    [Header("Other stats")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;

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

        //playerData = GetComponent<PlayerData>();
        playerInventory = GetComponent<PlayerInventory>();
        baseStats = actualStats = playerData.stats;
        currentHealth = actualStats.maxHealth;

        healthData.maxHealth = actualStats.maxHealth;
        healthData.currentHealth = actualStats.maxHealth;
        playerAgentData.moveSpeed = actualStats.moveSpeed;

        collector.SetRadius(CurrentMagnet);

    }

    private void Start()
    {
        playerInventory.AddYokai(playerData.startingWeapon);
        playerInventory.AddYokai(playerData.startingWeapon);

        experienceCap = levelRanges[0].experienceCapIncrease;

        GameManager.instance.currentHealthDisplay.text = "Max Health: " + CurrentMaxHealth;
        GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + CurrentRecovery;
        GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + CurrentMoveSpeed;
        GameManager.instance.currentPowerDisplay.text = "Power: " + CurrentPower;
        GameManager.instance.currentMagnetDisplay.text = "Magnet: " + CurrentMagnet;

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

   
    public void IncreaseExperience(int amount)
    {
        experience += amount;
        
        LevelUpChecker();
        UpdateExpBar();
    }

    void LevelUpChecker()
    {
        if(experience >= experienceCap)
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
            healthData.currentHealth += CurrentRecovery * Time.deltaTime;
            if (healthData.currentHealth > healthData.maxHealth)
                healthData.currentHealth = healthData.maxHealth;
        }
        CurrentHealth = healthData.currentHealth;
        healthData.UpdateHealthBar();

    }

    public void PlayerDie()
    {
        if (!GameManager.instance.isGameOver)
        {
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
        GameManager.instance.currentHealthDisplay.text = "Health: " + CurrentMaxHealth;
        GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + CurrentRecovery;
        GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + CurrentMoveSpeed;
        GameManager.instance.currentPowerDisplay.text = "Power: " + CurrentPower;
        GameManager.instance.currentMagnetDisplay.text = "Magnet: " + CurrentMagnet;
        playerAgentData.moveSpeed = actualStats.moveSpeed;
        healthData.maxHealth = actualStats.maxHealth;

    }

}
