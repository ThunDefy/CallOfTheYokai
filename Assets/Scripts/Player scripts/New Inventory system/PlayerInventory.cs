using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;
using static UnityEditor.Progress;
using Random = UnityEngine.Random;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class Slot
    {
        public YokaiData yokaiData;
        public Yokai yokai;
        public Image image;
        public int currentLevel=1;

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
            if(image != null)
            {
                image.sprite = null;
                image.enabled = false;
            }  
        }

        public bool IsEmpty() { return yokai == null; }
    }


    public List<Slot> weaponSlots = new List<Slot>(4);
    public List<Slot> passiveSlots = new List<Slot>(4);

    int activeWeaponIndx = 0;

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

    [System.Serializable]
    public class WeaponInfoUI
    {
        public GameObject weaponPanel;
        public Image weaponIcon;

        public TMP_Text weaponNameDisplay;
        public TMP_Text weaponDiscription;

        // Weapon active stats
        public TMP_Text damageStatDisplay;
        public TMP_Text areaStatDisplay;
        public TMP_Text speedStatDisplay;
        public TMP_Text cooldownStatDisplay;
        public TMP_Text knockbackStatDisplay;
        public TMP_Text pircingStatDisplay;

        // Weapon passive stats
        public TMP_Text PassiveEffect1Display;
        public TMP_Text PassiveEffect2Display;

        public Button changeButton;
        public Button nopeButton;

    }

    [Header("UI Elements")]
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();
    public List<WeaponInfoUI> changeUIOptions = new List<WeaponInfoUI>();
    public List<WeaponInfoUI> risingWeaponUI = new List<WeaponInfoUI>(2);

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

        //Определяем есть ли уже такое оружие
        for (int i = 0; i < weaponSlots.Capacity; i++)
        {
            if (!weaponSlots[i].IsEmpty())
            {
                if (weaponSlots[i].yokaiData.yokaiID == data.yokaiID)
                {
                    OnRisingYokai(i);
                    return -2;
                }

            }
        }

        // Ищем пустой слот
        for (int i = 0; i < weaponSlots.Capacity; i++)
        {
            if (weaponSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        if (slotNum < 0) // Если все слоты заняты
        {
            ChangingWeapon(data);
            return -1;
        }

        Type weaponType = Type.GetType(data.behaviour);

        if (weaponType != null) 
        {
            // Создаем объект оружия в WeaponParent
            GameObject go = Instantiate(data.yokaiPrefab);
            go.name = data.baseStats.name + " Controller";
            Weapon spawnedWeapon = (Weapon)go.AddComponent(weaponType);
            spawnedWeapon.transform.SetParent(GetComponentInChildren<WeaponParent>().transform);
            spawnedWeapon.transform.localPosition = Vector2.zero;
            spawnedWeapon.transform.localPosition += new Vector3(0.8f, 0f, 0f);
            spawnedWeapon.transform.rotation = Quaternion.identity;
            spawnedWeapon.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
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

            SetActiveWeapon(slotNum);
            return slotNum;
        }
        else
        {
            Debug.LogWarning(string.Format("Invalid weapon type specified for {0}.",data.name));
        }

        return -1;
    }

    private void ChangingWeapon(PlayerWeaponData data)
    {
        GameManager.instance.StartChangingWeapon();
        // Предварительно создадим объект с оружием

        Type weaponType = Type.GetType(data.behaviour);

        GameObject weapon = Instantiate(data.yokaiPrefab);
        weapon.name = data.baseStats.name + " Controller";
        Weapon w = (Weapon)weapon.AddComponent(weaponType);
        w.transform.SetParent(GetComponentInChildren<WeaponParent>().transform);
        w.transform.localPosition = Vector2.zero;
        w.transform.localPosition += new Vector3(0.8f, 0f, 0f);
        w.transform.rotation = Quaternion.identity;
        w.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        w.Initialise(data);
        w.OnEquip();

        GameObject pas = new GameObject(data.passiveEffectData.baseStats.name + " Passive");
        Passive p = pas.AddComponent<Passive>();
        p.Initialise(data.passiveEffectData);
        p.transform.SetParent(GetComponentInChildren<WeaponParent>().transform);
        p.transform.localPosition = Vector2.zero;


        // Определим уровень нового оружия
        int levelOfNewYokai=0;
        int activeWeaponCount=0;
        for (int i = 0; i < weaponSlots.Capacity; i++)
        {
            if (!weaponSlots[i].IsEmpty())
            {
                activeWeaponCount++;
                levelOfNewYokai+=Get(weaponSlots[i].yokaiData).currentLevel;
                
            }
        }
        int mod = Random.Range(-1, 2);
        levelOfNewYokai = ((int)levelOfNewYokai / activeWeaponCount) + mod;
        if (levelOfNewYokai <= 0) levelOfNewYokai = 1;

        // Теперь повысить уровень оружия случайным образом
        int randomUpgradeIndx=0;
        int randomValue;
        
        for (int i = 0; i < levelOfNewYokai-1; i++)
        {
            randomValue = Random.Range(0, 2);
            if(randomValue == 0)
            {
                randomUpgradeIndx = ((PlayerWeaponData)data).GetRandomLevelData();
                LevelUp(w, randomUpgradeIndx);
            }
            else
            {
                randomUpgradeIndx = (data.passiveEffectData).GetRandomLevelData();
                LevelUp(p, randomUpgradeIndx);
            }   
        }

        // Вывести его статистику
        changeUIOptions[0].weaponIcon.sprite = data.icon;
        changeUIOptions[0].weaponNameDisplay.text = data.baseStats.name + " Lvl "+ levelOfNewYokai;
        changeUIOptions[0].weaponDiscription.text = data.baseStats.description;
        changeUIOptions[0].damageStatDisplay.text = "Damage: " + w.currentStats.damage;
        changeUIOptions[0].areaStatDisplay.text = "Area: " + w.currentStats.area;
        changeUIOptions[0].speedStatDisplay.text = "Speed: " + w.currentStats.speed;
        changeUIOptions[0].cooldownStatDisplay.text = "Cooldown: " + w.currentStats.cooldown;
        changeUIOptions[0].knockbackStatDisplay.text = "Knockback: " + w.currentStats.knockback;
        changeUIOptions[0].pircingStatDisplay.text = "Pircing: " + w.currentStats.pircing;

        List<string> possibleBoost = p.GetBoostsInfo();
        if (possibleBoost.Count == 1) changeUIOptions[0].PassiveEffect1Display.text = possibleBoost[0];
        else{
            changeUIOptions[0].PassiveEffect1Display.text = possibleBoost[0];
            changeUIOptions[0].PassiveEffect2Display.text = possibleBoost[1];
        }

        // Вывести статистику уже имеющихся
        for(int i = 0; i< changeUIOptions.Count-1;i++)
        {
            changeUIOptions[i + 1] = GetWeaponInfoUI(changeUIOptions[i + 1],i);   
        }

        // Присвоить кнопке замене функцию по замене оружия (Удалить имеющиеся, поставить на его место новое)

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            int currentIndex = i;
            if (weaponSlots[i].yokai != null)
            {
                changeUIOptions[i + 1].changeButton.onClick.RemoveAllListeners();
                changeUIOptions[i + 1].changeButton.onClick.AddListener(() => ApplyChangeWeapon(currentIndex,w,p,data, levelOfNewYokai));
            }
        }

        // Присвоить кнопке отказа повышение уровня герою
        changeUIOptions[0].nopeButton.onClick.RemoveAllListeners();
        changeUIOptions[0].nopeButton.onClick.AddListener(() => CancelChangeWeapon(data));

    }

    private void ApplyChangeWeapon(int slotNum, Weapon w, Passive p, PlayerWeaponData data,int levelOfNewYokai)
    {
        // Удалить объекты со сцены
        weaponSlots[slotNum].yokai.transform.gameObject.SetActive(true);
        GameObject weaponForRemove = GameObject.Find(((PlayerWeaponData)weaponSlots[slotNum].yokaiData).baseStats.name + " Controller");
        if (weaponForRemove != null)
        {
            Destroy(weaponForRemove);
            print(((PlayerWeaponData)weaponSlots[slotNum].yokaiData).baseStats.name + " Controller has DESTROY"); 
        }


        GameObject passiveForRemove = GameObject.Find(((PassiveData)passiveSlots[slotNum].yokaiData).baseStats.name + " Passive");
        if (passiveForRemove != null)
        {
            Destroy(passiveForRemove);
            print(((PassiveData)passiveSlots[slotNum].yokaiData).baseStats.name + " Passive has DESTROY");
        }

        // Очистить слоты
        weaponSlots[slotNum].Clear();
        passiveSlots[slotNum].Clear();

        // На их место добавить новое оружие
        weaponSlots[slotNum].Assign(w, data);
        passiveSlots[slotNum].Assign(p, data.passiveEffectData);

        weaponSlots[slotNum].currentLevel = levelOfNewYokai;

        player.RecalculateStats();

        SetActiveWeapon(slotNum);
        if (GameManager.instance != null && GameManager.instance.changingWeapon) GameManager.instance.EndChangingWeapon();
        
    }

    private void CancelChangeWeapon(PlayerWeaponData data)
    {
        GameObject weaponForRemove = GameObject.Find(data.baseStats.name + " Controller");
        if (weaponForRemove != null)
        {
            Destroy(weaponForRemove);
        }


        GameObject passiveForRemove = GameObject.Find(data.passiveEffectData.baseStats.name + " Passive");
        if (passiveForRemove != null)
        {
            Destroy(passiveForRemove);
        }
        if (GameManager.instance != null && GameManager.instance.changingWeapon) GameManager.instance.EndChangingWeapon();
        player.IncreaseExperience(player.experienceCap);
        
    }

    private WeaponInfoUI GetWeaponInfoUI(WeaponInfoUI slotUI,int slotNum)
    {
        WeaponInfoUI weaponInfo = slotUI;
        weaponInfo.weaponPanel.SetActive(true);
        weaponInfo.weaponIcon.sprite = ((PlayerWeaponData)weaponSlots[slotNum].yokaiData).icon;
        weaponInfo.weaponNameDisplay.text = ((PlayerWeaponData)weaponSlots[slotNum].yokaiData).baseStats.name + " Lvl " + weaponSlots[slotNum].currentLevel;
        weaponInfo.weaponDiscription.text = ((PlayerWeaponData)weaponSlots[slotNum].yokaiData).baseStats.description;
        weaponInfo.damageStatDisplay.text = "Damage: " + ((Weapon)weaponSlots[slotNum].yokai).currentStats.damage;
        weaponInfo.areaStatDisplay.text = "Area: " + ((Weapon)weaponSlots[slotNum].yokai).currentStats.area;
        weaponInfo.speedStatDisplay.text = "Speed: " + ((Weapon)weaponSlots[slotNum].yokai).currentStats.speed;
        weaponInfo.cooldownStatDisplay.text = "Cooldown: " + ((Weapon)weaponSlots[slotNum].yokai).currentStats.cooldown;
        weaponInfo.knockbackStatDisplay.text = "Knockback: " + ((Weapon)weaponSlots[slotNum].yokai).currentStats.knockback;
        weaponInfo.pircingStatDisplay.text = "Pircing: " + ((Weapon)weaponSlots[slotNum].yokai).currentStats.pircing;
        List<string> boosts = ((Passive)passiveSlots[slotNum].yokai).GetBoostsInfo();
        if(boosts.Count == 1) weaponInfo.PassiveEffect1Display.text = boosts[0];
        else
        {
            weaponInfo.PassiveEffect1Display.text = boosts[0];
            weaponInfo.PassiveEffect2Display.text = boosts[1];
        }
        return weaponInfo;
    }

    private void CompareWeaponInfo(WeaponInfoUI oldWeaponInfo, WeaponInfoUI newWeaponInfo)
    {
        if (oldWeaponInfo != null && newWeaponInfo != null)
        {
            // Проходим по каждому текстовому полю и сравниваем их
            foreach (var field in typeof(WeaponInfoUI).GetFields())
            {
                // Проверяем, что поле является текстовым
                if (field.FieldType == typeof(TMP_Text))
                {
                    TMP_Text oldText = (TMP_Text)field.GetValue(oldWeaponInfo);
                    TMP_Text newText = (TMP_Text)field.GetValue(newWeaponInfo);

                    // Если текст в полях отличается, меняем цвет на светло-зеленый
                    if (oldText.text != newText.text)
                    {
                        newText.color = new Color(90f / 255f, 255f / 255f, 115f / 255f, 1f);
                    }
                    else
                    {
                        newText.color = Color.white;
                    }
                }
            }
        }
    }
    private void OnRisingYokai(int slotNum)
    {
        // Вызов экрана возвышения оружия, показанны данные до и после возвышения
        GameManager.instance.StartRisingWeapon();
        

        risingWeaponUI[0] =GetWeaponInfoUI(risingWeaponUI[0],slotNum);
        weaponSlots[slotNum].yokai.RisingUpYokai();
        WeaponInfoUI NewInfo = GetWeaponInfoUI(risingWeaponUI[1], slotNum);

        CompareWeaponInfo(risingWeaponUI[0], NewInfo);
        risingWeaponUI[1] = NewInfo;

        risingWeaponUI[0].changeButton.onClick.RemoveAllListeners();
        risingWeaponUI[0].changeButton.onClick.AddListener(() => GameManager.instance.EndRisingWeapon());


    }

    public bool LevelUp(YokaiData data, int upgradeIdnx)
    {
        Yokai yokai = Get(data);
        if (yokai) return LevelUp(yokai, upgradeIdnx);
        return false;
    }

    // Увеличить уровень выбранного Йокая
    public bool LevelUp(Yokai yokai, int upgradeIdnx, int slotIndx = -1)
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
        if (slotIndx >= 0)
        {
            print("slotIndx = " + slotIndx);
            weaponSlots[slotIndx].currentLevel++;
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

                /////////////// Улучшение характеристик атаки
                randomUpgradeWeaponIndx1 = ((PlayerWeaponData)weaponSlots[i].yokaiData).GetRandomLevelData();
                Yokai yokai = weaponSlots[i].yokai;
                Weapon.Stats upgradeWeaponChoise1 = ((PlayerWeaponData)weaponSlots[i].yokaiData).GetLevelData(randomUpgradeWeaponIndx1);
                upgradeUIOptions[currentIndex].upgrade1YokaiDescriptionDisplay.text = upgradeWeaponChoise1.description;
                upgradeUIOptions[currentIndex].upgrade1YokaiNameDisplay.text = upgradeWeaponChoise1.name;
                upgradeUIOptions[currentIndex].upgrade1YokaiButton.onClick.RemoveAllListeners();
                upgradeUIOptions[currentIndex].upgrade1YokaiButton.onClick.AddListener(() => LevelUp(yokai, randomUpgradeWeaponIndx1, currentIndex));

                // 2й вариант улучшения характеристик атаки
                do { randomUpgradeWeaponIndx2 = ((PlayerWeaponData)weaponSlots[i].yokaiData).GetRandomLevelData(); }while(randomUpgradeWeaponIndx1 == randomUpgradeWeaponIndx2);
                Weapon.Stats upgradeWeaponChoise2 = ((PlayerWeaponData)weaponSlots[i].yokaiData).GetLevelData(randomUpgradeWeaponIndx2);
                upgradeUIOptions[currentIndex].upgrade2YokaiDescriptionDisplay.text = upgradeWeaponChoise2.description;
                upgradeUIOptions[currentIndex].upgrade2YokaiNameDisplay.text = upgradeWeaponChoise2.name;
                upgradeUIOptions[currentIndex].upgrade2YokaiButton.onClick.RemoveAllListeners();
                upgradeUIOptions[currentIndex].upgrade2YokaiButton.onClick.AddListener(() => LevelUp(yokai, randomUpgradeWeaponIndx2, currentIndex));


                /////////////// Или улучшение Пассивного эффекта
                randomUpgradePassiveIndx1 = ((PassiveData)passiveSlots[i].yokaiData).GetRandomLevelData();
                Yokai yokaiPassive = passiveSlots[i].yokai;
                Passive.Modifier upgradePassiveChoise1 = ((PassiveData)passiveSlots[i].yokaiData).GetLevelData(randomUpgradePassiveIndx1);
                upgradeUIOptions[currentIndex].upgrade1PassiveDescriptionDisplay.text = upgradePassiveChoise1.description;
                upgradeUIOptions[currentIndex].upgrade1PassiveNameDisplay.text = upgradePassiveChoise1.name;
                upgradeUIOptions[currentIndex].upgrade1PassiveButton.onClick.RemoveAllListeners();
                upgradeUIOptions[currentIndex].upgrade1PassiveButton.onClick.AddListener(() => LevelUp(yokaiPassive, randomUpgradePassiveIndx1, currentIndex));

                // 2й вариант улучшения Пассивного эффекта
                do { randomUpgradePassiveIndx2 = ((PassiveData)passiveSlots[i].yokaiData).GetRandomLevelData(); } while (randomUpgradePassiveIndx1 == randomUpgradePassiveIndx2);
                Passive.Modifier upgradePassiveChoise2 = ((PassiveData)passiveSlots[i].yokaiData).GetLevelData(randomUpgradePassiveIndx2);
                upgradeUIOptions[currentIndex].upgrade2PassiveDescriptionDisplay.text = upgradePassiveChoise2.description;
                upgradeUIOptions[currentIndex].upgrade2PassiveNameDisplay.text = upgradePassiveChoise2.name;
                upgradeUIOptions[currentIndex].upgrade2PassiveButton.onClick.RemoveAllListeners();
                upgradeUIOptions[currentIndex].upgrade2PassiveButton.onClick.AddListener(() => LevelUp(yokaiPassive, randomUpgradePassiveIndx2, currentIndex));
            }           
        }
    }

    public void SwapActiveWeapon()
    {
        int newActiveWeaponIndx=0;
        if (activeWeaponIndx+1 < weaponSlots.Capacity)
        {
            if (weaponSlots[activeWeaponIndx + 1].IsEmpty())
            {
                newActiveWeaponIndx = 0;
            }
            else
            {
                newActiveWeaponIndx = activeWeaponIndx+1;
            }
        }else newActiveWeaponIndx = 0;
        SetActiveWeapon(newActiveWeaponIndx);
    }

    void SetActiveWeapon(int weaponIndx)
    {
        //print("Меняю " + activeWeaponIndx + " На "+ weaponIndx);
        weaponSlots[activeWeaponIndx].yokai.transform.gameObject.SetActive(false);
        weaponSlots[weaponIndx].yokai.transform.gameObject.SetActive(true);
        activeWeaponIndx=weaponIndx;
    }

}
