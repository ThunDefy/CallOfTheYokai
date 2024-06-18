using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProgression : MonoBehaviour
{
    [System.Serializable]
    public class PlayerStatsUpgradeUI 
    {
        public TMP_Text statNameDisplay;
        public TMP_Text currentValueDisplay;
        public TMP_Text currentPriceDisplay;
        public GameObject changeButtons;  
        public bool isSpecialStats;
        public float increaseValue;
        public int increasePrice;
        public int increaseCup;
        public float maxValue;
        [HideInInspector]
        public Button plusButton;
        [HideInInspector]
        public Button minusButton;
        [HideInInspector]
        public PlayerProgression playerProgressionData;
        [HideInInspector]
        public float defaultValue;
        private float currentValue;
        private float newValue;
        private int currentPrice;
        private int currentSoulsCount;
        public bool isChanged = false;
        public void SetButtons()
        {
            if (changeButtons)
            {
                plusButton = changeButtons.transform.GetChild(0).GetComponent<Button>();
                minusButton = changeButtons.transform.GetChild(1).GetComponent<Button>();

                plusButton.onClick.AddListener(IncreaseStat);
                minusButton.onClick.AddListener(CancelLastIncrease);
            }
        }

        public void SetCurrentValue(float value)
        {
            //print(defaultValue);
            currentValue = value;
            newValue = currentValue;
            currentPrice = increasePrice;
            RecalculatePrice();          
        }
        public float GetNewValue()
        {
            return newValue;
        }

        public void IncreaseStat()
        {

            if (currentSoulsCount >= currentPrice)
            {
                newValue += increaseValue;
                playerProgressionData.SoulsPay(currentPrice, isSpecialStats);
                currentPrice += increaseCup;
                UpdateUI();    
            }
            isChanged = true;
        }
        public void CancelLastIncrease()
        {

            if (newValue > currentValue && isChanged)
            {
                newValue -= increaseValue;
                currentPrice -= increaseCup;
                playerProgressionData.SoulsPay(-currentPrice, isSpecialStats);
                //isChanged = false;
            }
            UpdateUI();
        }

        public void CancelAllIncrease()
        {
            if (isChanged)
            {
                newValue = currentValue;
                currentPrice = increasePrice;
                RecalculatePrice();
                UpdateData();
                UpdateUI();
                
                isChanged =false;
            }           
        }
        public void UpdateUI()
        {
            currentValueDisplay.text = newValue.ToString();
            currentPriceDisplay.text = currentPrice.ToString();

            if (newValue != currentValue)
            {
                minusButton.interactable = true;
                currentValueDisplay.color = Color.green;
            }
            else
            {
                minusButton.interactable = false;
                currentValueDisplay.color = Color.black;
            }

            if (currentSoulsCount < currentPrice || newValue + increaseValue > maxValue)
            {
                AvailableIncrease(false);
                if(newValue + increaseValue > maxValue)
                {
                    currentPriceDisplay.text = "max";
                }
            }else AvailableIncrease(true);
        }

        public void AvailableIncrease(bool avaible)
        {
            plusButton.interactable = avaible;
        }

        public void UpdateData()
        {
            if (isSpecialStats) 
                currentSoulsCount = playerProgressionData.currentRareSoulCount;
            else
                currentSoulsCount = playerProgressionData.currentCommonSoulCount;
            UpdateUI();
        }

        private void RecalculatePrice()
        {
            currentPrice += Mathf.FloorToInt((currentValue - defaultValue) / increaseValue) * increaseCup;
        }
    }

    public TMP_Text rareSoulsCountDisplay;
    public TMP_Text commonSoulsCountDisplay;

    public PlayerData defaultPlayerStats;

    public PlayerStatsUpgradeUI[] playerCommonStatsUpgradeUI;
    public PlayerStatsUpgradeUI[] playerSpecialStatsUpgradeUI;

    private PlayerData.Stats currentPlayerStats;
    private PlayerData.Stats newPlayerStats;

    [HideInInspector]
    public int currentCommonSoulCount, currentRareSoulCount;

    private void Start()
    {
        SetCurrentValues();

        if (playerCommonStatsUpgradeUI!=null && playerSpecialStatsUpgradeUI != null)
        {
            foreach (var item in playerCommonStatsUpgradeUI)
            {
                item.SetButtons();
                item.playerProgressionData = this;
                item.UpdateData();
            }
            foreach (var item in playerSpecialStatsUpgradeUI)
            {
                item.SetButtons();
                item.playerProgressionData = this;
                item.UpdateData();
            }
        }
        UpdateSoulsDisplays();
    }

    public void SoulsPay(int amount, bool isRare = false)
    {
        if (!isRare)
            currentCommonSoulCount -= amount;
        else
            currentRareSoulCount -= amount;

        if (playerCommonStatsUpgradeUI != null && playerSpecialStatsUpgradeUI != null)
        {
            foreach (var item in playerCommonStatsUpgradeUI)
            {
                item.UpdateData();
            }
            foreach (var item in playerSpecialStatsUpgradeUI)
            {
                item.UpdateData();
            }
        }
        UpdateSoulsDisplays();
    }

    private void SetCurrentValues()
    {
        currentPlayerStats = SaveAndLoadManager.LoadPlayerData();
        newPlayerStats = currentPlayerStats;
        playerCommonStatsUpgradeUI[0].defaultValue = defaultPlayerStats.stats.maxHealth;
        playerCommonStatsUpgradeUI[0].SetCurrentValue(currentPlayerStats.maxHealth);
        playerCommonStatsUpgradeUI[1].defaultValue = defaultPlayerStats.stats.recovery;
        playerCommonStatsUpgradeUI[1].SetCurrentValue(currentPlayerStats.recovery);
        playerCommonStatsUpgradeUI[2].defaultValue = defaultPlayerStats.stats.armor;
        playerCommonStatsUpgradeUI[2].SetCurrentValue(currentPlayerStats.armor);
        playerCommonStatsUpgradeUI[3].defaultValue = defaultPlayerStats.stats.moveSpeed;
        playerCommonStatsUpgradeUI[3].SetCurrentValue(currentPlayerStats.moveSpeed);
        playerCommonStatsUpgradeUI[4].defaultValue = defaultPlayerStats.stats.area;
        playerCommonStatsUpgradeUI[4].SetCurrentValue(currentPlayerStats.area);
        playerCommonStatsUpgradeUI[5].defaultValue = defaultPlayerStats.stats.speed;
        playerCommonStatsUpgradeUI[5].SetCurrentValue(currentPlayerStats.speed);
        playerCommonStatsUpgradeUI[6].defaultValue = defaultPlayerStats.stats.cooldown;
        playerCommonStatsUpgradeUI[6].SetCurrentValue(currentPlayerStats.cooldown);
        playerCommonStatsUpgradeUI[7].defaultValue = defaultPlayerStats.stats.growth;
        playerCommonStatsUpgradeUI[7].SetCurrentValue(currentPlayerStats.growth);
        playerCommonStatsUpgradeUI[8].defaultValue = defaultPlayerStats.stats.magnet;
        playerCommonStatsUpgradeUI[8].SetCurrentValue(currentPlayerStats.magnet);
        playerSpecialStatsUpgradeUI[0].defaultValue = defaultPlayerStats.stats.power;
        playerSpecialStatsUpgradeUI[0].SetCurrentValue(currentPlayerStats.power);
        playerSpecialStatsUpgradeUI[1].defaultValue = defaultPlayerStats.stats.luck;
        playerSpecialStatsUpgradeUI[1].SetCurrentValue(currentPlayerStats.luck);
        playerSpecialStatsUpgradeUI[2].defaultValue = defaultPlayerStats.stats.availableSlots;
        playerSpecialStatsUpgradeUI[2].SetCurrentValue(currentPlayerStats.availableSlots);    
        currentCommonSoulCount = currentPlayerStats.commonSouls; 
        currentRareSoulCount = currentPlayerStats.specialSouls;
    }

    private void UpdateSoulsDisplays()
    {
        rareSoulsCountDisplay.text = currentRareSoulCount.ToString();
        commonSoulsCountDisplay.text = currentCommonSoulCount.ToString();
    }

    public void SaveChanges()
    {
        newPlayerStats.maxHealth = playerCommonStatsUpgradeUI[0].GetNewValue();
        newPlayerStats.recovery = playerCommonStatsUpgradeUI[1].GetNewValue();
        newPlayerStats.armor = playerCommonStatsUpgradeUI[2].GetNewValue();
        newPlayerStats.moveSpeed = playerCommonStatsUpgradeUI[3].GetNewValue();
        newPlayerStats.area = playerCommonStatsUpgradeUI[4].GetNewValue();
        newPlayerStats.speed = playerCommonStatsUpgradeUI[5].GetNewValue();
        newPlayerStats.cooldown = playerCommonStatsUpgradeUI[6].GetNewValue();
        newPlayerStats.growth = playerCommonStatsUpgradeUI[7].GetNewValue();
        newPlayerStats.magnet = playerCommonStatsUpgradeUI[8].GetNewValue();

        newPlayerStats.power = playerSpecialStatsUpgradeUI[0].GetNewValue();
        newPlayerStats.luck = playerSpecialStatsUpgradeUI[1].GetNewValue();
        newPlayerStats.availableSlots = (int)playerSpecialStatsUpgradeUI[2].GetNewValue();

        newPlayerStats.specialSouls = currentRareSoulCount;
        newPlayerStats.commonSouls = currentCommonSoulCount;

        SaveAndLoadManager.SavePlayerData(newPlayerStats);
        SetCurrentValues();
        UpdateSoulsDisplays();

       

        foreach (var item in playerCommonStatsUpgradeUI)
        {
            item.isChanged = false;
            item.UpdateUI();
        }
        foreach (var item in playerSpecialStatsUpgradeUI)
        {
            item.isChanged = false;
            item.UpdateUI();
        }
    }

    public void UndoChanges()
    {
        currentRareSoulCount = currentPlayerStats.specialSouls;
        currentCommonSoulCount = currentPlayerStats.commonSouls;

        if (playerCommonStatsUpgradeUI != null && playerSpecialStatsUpgradeUI != null)
        {
            foreach (var item in playerCommonStatsUpgradeUI)
            {
                item.CancelAllIncrease();
            }
            foreach (var item in playerSpecialStatsUpgradeUI)
            {
                item.CancelAllIncrease();
            }
        }
        UpdateSoulsDisplays();

    }
}
