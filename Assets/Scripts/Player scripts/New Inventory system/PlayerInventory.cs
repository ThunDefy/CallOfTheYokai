using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class Slot
    {
        public YokaiData yokaiData;
        public Yokai yokai;
        public Image image;

        public void Assign(Yokai assignedItem, YokaiData assignedData)
        {
            yokaiData = assignedData;
            yokai = assignedItem;
            if (yokai is Weapon)
            {
                Weapon w = yokai as Weapon;
                image.enabled = true;
                image.sprite = w.data.icon;
            }
            else
            {
                Passive p = yokai as Passive;
            }
            Debug.Log(string.Format("Assigned {0} to player.", yokai.name));
        }

        public void Clear()
        {
            yokaiData = null;
            yokai = null;
            image.enabled = false;
            image.sprite = null;
        }

        public bool IsEmpty() { return yokai == null; }
    }


    public List<Slot> weaponSlots = new List<Slot>(4);
    public List<Slot> passiveSlots = new List<Slot>(4);

    [System.Serializable]
    public class UpgradeUI
    {
        public GameObject upgradeWeaponPanel;
        public Image upgradeIcon;
        // Choise 1
        public TMP_Text upgrade1YokaiNameDisplay;
        public TMP_Text upgrade1YokaiDescriptionDisplay;
        public Button upgrade1YokaiButton;

        public TMP_Text upgrade1PassiveNameDisplay;
        public TMP_Text upgrade1PassiveDescriptionDisplay;
        public Button upgrade1PassiveButton;
        // Choise 2
        public TMP_Text upgrade2YokaiNameDisplay;
        public TMP_Text upgrade2YokaiDescriptionDisplay;
        public Button upgrade2YokaiButton;

        public TMP_Text upgrade2PassiveNameDisplay;
        public TMP_Text upgrade2PassiveDescriptionDisplay;
        public Button upgrade2PassiveButton;
    }

    [Header("UI Elements")]
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();    

    PlayerStats player;

    void Awake()
    {
        player = GetComponent<PlayerStats>();
    }

    public bool Has(YokaiData type) { return Get(type); }

    public Yokai Get(YokaiData type)
    {
        if (type is PlayerWeaponData) return Get(type as PlayerWeaponData);
        else if (type is PassiveData) return Get(type as PassiveData);
        return null;
    }
    public Passive Get(PassiveData type)
    {
        foreach (Slot s in passiveSlots)
        {
            Passive p = s.yokai as Passive;
            if (p && p.data == type)
                return p;
        }
        return null;
    }

    public Weapon Get(PlayerWeaponData type)
    {
        foreach (Slot s in weaponSlots)
        {
            Weapon w = s.yokai as Weapon;
            if (w && w.data == type)
                return w;
        }
        return null;
    }

    public int AddYokai(PlayerWeaponData data)
    {
        int slotNum = -1;

        // Ищем пустой слот
        for (int i = 0; i < weaponSlots.Capacity; i++)
        {
            if (weaponSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }
        if (slotNum < 0) return slotNum;

        Type weaponType = Type.GetType(data.behaviour);

        if (weaponType != null) 
        {
            // Создаем объект оружия в WeaponParent
            GameObject go = Instantiate(data.yokaiPrefab);
            go.name = data.baseStats.name + " Controller";
            Weapon spawnedWeapon = (Weapon)go.AddComponent(weaponType);
            spawnedWeapon.transform.SetParent(GetComponentInChildren<WeaponParent>().transform);
            spawnedWeapon.transform.localPosition = Vector2.zero;
            spawnedWeapon.transform.position += new Vector3(0.8f, 0f, 0f);
            spawnedWeapon.Initialise(data);
            spawnedWeapon.OnEquip();

            // Добавляем оружие в инвентарь
            weaponSlots[slotNum].Assign(spawnedWeapon, data);

            // Так же добавляем пассивный эффект 

            GameObject pas = new GameObject(data.passiveEffectData.baseStats.name + " Passive");
            Passive p = pas.AddComponent<Passive>();
            p.Initialise(data.passiveEffectData);
            p.transform.SetParent(GetComponentInChildren<WeaponParent>().transform); 
            p.transform.localPosition = Vector2.zero;

            passiveSlots[slotNum].Assign(p, data.passiveEffectData);

            player.RecalculateStats();

            return slotNum;
        }
        else
        {
            Debug.LogWarning(string.Format("Invalid weapon type specified for {0}.",data.name));
        }

        return -1;
    }

    public bool LevelUp(YokaiData data, int upgradeIdnx)
    {
        Yokai yokai = Get(data);
        if (yokai) return LevelUp(yokai, upgradeIdnx);
        return false;
    }

    // Увеличить уровень выбранного Йокая
    public bool LevelUp(Yokai yokai, int upgradeIdnx)
    {
        // Tries to level up the yokai.
        if (!yokai.DoLevelUp(upgradeIdnx))
        {
            return false;
        }

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }

        if (yokai is Passive)
        {
            player.RecalculateStats();
        }
        return true;
    }

    // Проверка на наличие свободных слотов
    int GetSlotsLeft(List<Slot> slots)
    {

        int count = 0;
        foreach (Slot s in slots)
        {
            if (s.IsEmpty()) count++;
        }
        return count;
    }

    // Определяет, какие параметры апгрейда должны появиться


    void ApplyUpgradeOptions()
    {

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            
            int currentIndex = i;

            if (weaponSlots[i].yokai!=null) 
            {
                int randomUpgradeWeaponIndx1, randomUpgradeWeaponIndx2;
                int randomUpgradePassiveIndx1, randomUpgradePassiveIndx2;
                upgradeUIOptions[currentIndex].upgradeWeaponPanel.SetActive(true);
                upgradeUIOptions[currentIndex].upgradeIcon.sprite = weaponSlots[i].yokaiData.icon;

                // Улучшение характеристик атаки
                randomUpgradeWeaponIndx1 = ((PlayerWeaponData)weaponSlots[i].yokaiData).GetRandomLevelData();
                Yokai yokai = weaponSlots[i].yokai;
                Weapon.Stats upgradeWeaponChoise1 = ((PlayerWeaponData)weaponSlots[i].yokaiData).GetLevelData(randomUpgradeWeaponIndx1);
                upgradeUIOptions[currentIndex].upgrade1YokaiDescriptionDisplay.text = upgradeWeaponChoise1.description;
                upgradeUIOptions[currentIndex].upgrade1YokaiNameDisplay.text = upgradeWeaponChoise1.name;
                upgradeUIOptions[currentIndex].upgrade1YokaiButton.onClick.RemoveAllListeners();
                upgradeUIOptions[currentIndex].upgrade1YokaiButton.onClick.AddListener(() => LevelUp(yokai, randomUpgradeWeaponIndx1));

                // 2й вариант улучшения
                do { randomUpgradeWeaponIndx2 = ((PlayerWeaponData)weaponSlots[i].yokaiData).GetRandomLevelData(); }while(randomUpgradeWeaponIndx1 == randomUpgradeWeaponIndx2);
                Weapon.Stats upgradeWeaponChoise2 = ((PlayerWeaponData)weaponSlots[i].yokaiData).GetLevelData(randomUpgradeWeaponIndx2);
                upgradeUIOptions[currentIndex].upgrade2YokaiDescriptionDisplay.text = upgradeWeaponChoise2.description;
                upgradeUIOptions[currentIndex].upgrade2YokaiNameDisplay.text = upgradeWeaponChoise2.name;
                upgradeUIOptions[currentIndex].upgrade2YokaiButton.onClick.RemoveAllListeners();
                upgradeUIOptions[currentIndex].upgrade2YokaiButton.onClick.AddListener(() => LevelUp(yokai, randomUpgradeWeaponIndx2));


                // Или улучшение Пассивного эффекта

                randomUpgradePassiveIndx1 = ((PassiveData)passiveSlots[i].yokaiData).GetRandomLevelData();
                Yokai yokaiPassive = passiveSlots[i].yokai;
                Passive.Modifier upgradePassiveChoise1 = ((PassiveData)passiveSlots[i].yokaiData).GetLevelData(randomUpgradePassiveIndx1);
                upgradeUIOptions[currentIndex].upgrade1PassiveDescriptionDisplay.text = upgradePassiveChoise1.description;
                upgradeUIOptions[currentIndex].upgrade1PassiveNameDisplay.text = upgradePassiveChoise1.name;
                upgradeUIOptions[currentIndex].upgrade1PassiveButton.onClick.RemoveAllListeners();
                upgradeUIOptions[currentIndex].upgrade1PassiveButton.onClick.AddListener(() => LevelUp(yokaiPassive, randomUpgradePassiveIndx1));

                // 2й вариант улучшения
                do { randomUpgradePassiveIndx2 = ((PassiveData)passiveSlots[i].yokaiData).GetRandomLevelData(); } while (randomUpgradePassiveIndx1 == randomUpgradePassiveIndx2);
                Passive.Modifier upgradePassiveChoise2 = ((PassiveData)passiveSlots[i].yokaiData).GetLevelData(randomUpgradePassiveIndx2);
                upgradeUIOptions[currentIndex].upgrade2PassiveDescriptionDisplay.text = upgradePassiveChoise2.description;
                upgradeUIOptions[currentIndex].upgrade2PassiveNameDisplay.text = upgradePassiveChoise2.name;
                upgradeUIOptions[currentIndex].upgrade2PassiveButton.onClick.RemoveAllListeners();
                upgradeUIOptions[currentIndex].upgrade2PassiveButton.onClick.AddListener(() => LevelUp(yokaiPassive, randomUpgradePassiveIndx2));



            }
        }
    }

}
