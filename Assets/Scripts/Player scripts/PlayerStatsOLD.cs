using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsOLD : MonoBehaviour
{
    [HideInInspector]
    public Health healthData;
    [HideInInspector]
    public Agent playerData;
    [HideInInspector]
    public WeaponParent weaponParent;
    

    PlayerCollector collector;

    //public List<GameObject> spawnedWeapons;

    InventoryManager inventory;
    public int weaponIndex;

    public GameObject startingWeapon;
    

    [Header("Player stats")]
    public PlayerData player;

    float currentRecovery;
    float currentMagnet;
    float currentPower;
    float currentMoveSpeed;
    float currentHealth;
    float currentMaxHealth;

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
        get { return currentMaxHealth; }
        set
        {
            if (currentMaxHealth != value)
            {
                currentMaxHealth = value;
                healthData.maxHealth = currentMaxHealth;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = "Max Health: " + currentMaxHealth;
                }

            }
        }
    }

    public float CurrentRecovery
    {
        get { return currentRecovery; }
        set
        {
            if (currentRecovery != value)
            {
                currentRecovery = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + currentRecovery;
                }
            }
        }
    }

    public float CurrentMagnet
    {
        get { return currentMagnet; }
        set
        {
            if (currentMagnet != value)
            {
                currentMagnet = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMagnetDisplay.text = "Magnet: " + currentMagnet;
                }
            }
        }
    }

    public float CurrentPower
    {
        get { return currentPower; }
        set
        {
            if (currentPower != value)
            {
                currentPower = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentPowerDisplay.text = "Power: " + currentPower;
                }
            }
        }
    }
    public float CurrentMoveSpeed
    {
        get { return currentMoveSpeed; }
        set
        {
            if (currentMoveSpeed != value)
            {
                currentMoveSpeed = value;
                playerData.moveSpeed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + currentMoveSpeed;
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
        playerData = GetComponent<Agent>();
        weaponParent = GetComponentInChildren<WeaponParent>();
        inventory = GetComponent<InventoryManager>();
        collector = GetComponentInChildren<PlayerCollector>();

        CurrentRecovery = player.stats.recovery;
        CurrentMagnet = player.stats.magnet;
        CurrentPower = player.stats.power;
        CurrentMaxHealth = player.stats.maxHealth;
        CurrentHealth = player.stats.maxHealth;
        CurrentMoveSpeed = player.stats.moveSpeed;
        collector.SetRadius(CurrentMagnet);
        SpawnWeapon(startingWeapon);

    }

    private void Start()
    {
        experienceCap = levelRanges[0].experienceCapIncrease;
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

    public void SpawnWeapon(GameObject weapon)
    {
        if(weaponIndex >= inventory.weaponSlots.Count)
        {
            Debug.LogError("Inventory full");
            return;
        }

        GameObject spawnedWeapon = Instantiate(weapon, weaponParent.transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(weaponParent.transform);

        Vector3 weaponLocalPosition = spawnedWeapon.transform.localPosition;
        weaponLocalPosition.x += 0.8f;
        spawnedWeapon.transform.localPosition = weaponLocalPosition;

        inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponControllers>(), spawnedWeapon.GetComponent<PassiveEffect>());
        weaponIndex++;

    }

    public void PlayerDie()
    {
        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssignLevelReachedUI(level);
            //GameManager.instance.AssignChosenWeaponsUI(inventory.weaponUISlots);
            GameManager.instance.GameOver();
            
        }
    }

    public void RecalculateStats() { }

}
