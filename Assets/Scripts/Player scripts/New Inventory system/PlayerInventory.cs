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
        public Yokai yokai;
        public Image image;

        public void Assign(Yokai assignedItem)
        {
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
                //image.enabled = true;
                //image.sprite = p.data.icon;
            }
            Debug.Log(string.Format("Assigned {0} to player.", yokai.name));
        }

        public void Clear()
        {
            yokai = null;
            image.enabled = false;
            image.sprite = null;
        }

        public bool IsEmpty() { return yokai == null; }
    }


    public List<Slot> weaponSlots = new List<Slot>(6);
    public List<Slot> passiveSlots = new List<Slot>(6);

    [System.Serializable]
    public class UpgradeUI
    {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    [Header("UI Elements")]
    public List<PlayerWeaponData> availableWeapons = new List<PlayerWeaponData>();    //List of upgrade options for weapons
    public List<PassiveData> availablePassives = new List<PassiveData>(); //List of upgrade options for passive items
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();    //List of ui for upgrade options present in the scene

    PlayerStats player;

    void Start()
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
    public bool Remove(PlayerWeaponData data, bool removeUpgradeAvailability = false)
    {
        // Remove this weapon from the upgrade pool.
        if (removeUpgradeAvailability) availableWeapons.Remove(data);

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            Weapon w = weaponSlots[i].yokai as Weapon;
            if (w.data == data)
            {
                weaponSlots[i].Clear();
                //w.OnUnequip();
                Destroy(w.gameObject);
                return true;
            }
        }

        return false;
    }
    public bool Remove(PassiveData data, bool removeUpgradeAvailability = false)
    {
        // Remove this passive from the upgrade pool.
        if (removeUpgradeAvailability) availablePassives.Remove(data);

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            Passive p = weaponSlots[i].yokai as Passive;
            if (p.data == data)
            {
                weaponSlots[i].Clear();
                //p.OnUnequip();
                Destroy(p.gameObject);
                return true;
            }
        }

        return false;
    }
    public bool Remove(YokaiData data, bool removeUpgradeAvailability = false)
    {
        if (data is PassiveData) return Remove(data as PassiveData, removeUpgradeAvailability);
        else if (data is PlayerWeaponData) return Remove(data as PlayerWeaponData, removeUpgradeAvailability);
        return false;
    }

    public int AddYokai(PlayerWeaponData data)
    {
        int slotNum = -1;

        // Try to find an empty slot.
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

        if (weaponType != null) //yokaiPrefab
        {
            // Spawn the weapon GameObject.
            GameObject go = Instantiate(data.yokaiPrefab);
            go.name = data.baseStats.name + " Controller";
            //GameObject go = new GameObject(data.baseStats.name + " Controller");
            Weapon spawnedWeapon = (Weapon)go.AddComponent(weaponType);
            spawnedWeapon.transform.SetParent(GetComponentInChildren<WeaponParent>().transform); //Set the weapon to be a child of the player
            spawnedWeapon.transform.localPosition = Vector2.zero;
            spawnedWeapon.transform.position += new Vector3(0.8f, 0f, 0f);
            spawnedWeapon.Initialise(data);
            spawnedWeapon.OnEquip();

            // Assign the weapon to the slot.
            weaponSlots[slotNum].Assign(spawnedWeapon);

            // Close the level up UI if it is on.
            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
                GameManager.instance.EndLevelUp();

            Add(data.passiveEffectData);

            return slotNum;
        }
        else
        {
            Debug.LogWarning(string.Format("Invalid weapon type specified for {0}.",data.name));
        }

        return -1;
    }
    public int Add(PassiveData data)
    {
        int slotNum = -1;

        // Try to find an empty slot.
        for (int i = 0; i < passiveSlots.Capacity; i++)
        {
            if (passiveSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }
        if (slotNum < 0) return slotNum;

        GameObject go = new GameObject(data.baseStats.name + " Passive");
        Passive p = go.AddComponent<Passive>();
        p.Initialise(data);
        p.transform.SetParent(GetComponentInChildren<WeaponParent>().transform); //Set the weapon to be a child of the player
        p.transform.localPosition = Vector2.zero;

        // Assign the passive to the slot.
        passiveSlots[slotNum].Assign(p);

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        player.RecalculateStats();

        return slotNum;
    }
    //public int Add(YokaiData data)
    //{
    //    if (data is PlayerWeaponData) return Add(data as PlayerWeaponData);
    //    else if (data is PassiveData) return Add(data as PassiveData);
    //    return -1;
    //}

    // Overload so that we can use both ItemData or Item to level up an
    // yokai in the inventory.
    public bool LevelUp(YokaiData data)
    {
        Yokai yokai = Get(data);
        if (yokai) return LevelUp(yokai);
        return false;
    }

    // Levels up a selected weapon in the player inventory.
    public bool LevelUp(Yokai yokai)
    {
        // Tries to level up the yokai.
        if (!yokai.DoLevelUp())
        {
            return false;
        }

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }

        if (yokai is Passive) player.RecalculateStats();
        return true;
    }

    // Checks a list of slots to see if there are any slots left.
    int GetSlotsLeft(List<Slot> slots)
    {

        int count = 0;
        foreach (Slot s in slots)
        {
            if (s.IsEmpty()) count++;
        }
        return count;
    }

    // Determines what upgrade options should appear.
    void ApplyUpgradeOptions()
    {

        List<YokaiData> availableUpgrades = new List<YokaiData>(availableWeapons.Count + availablePassives.Count);
        List<YokaiData> allPossibleUpgrades = new List<YokaiData>(availableWeapons);
        allPossibleUpgrades.AddRange(availablePassives);

        int weaponSlotsLeft = GetSlotsLeft(weaponSlots);
        int passiveSlotsLeft = GetSlotsLeft(passiveSlots);

        foreach (YokaiData data in allPossibleUpgrades)
        {
            // If a weapon of this type exists, allow for the upgrade if the
            // level of the weapon is not already maxed out.
            Yokai obj = Get(data);
            if (obj)
            {
                if (obj.currentLevel < data.maxLevel) availableUpgrades.Add(data);
            }
            else
            {
                // If we don't have this yokai in the inventory yet, check if
                // we still have enough slots to take new items.
                if (data is PlayerWeaponData && weaponSlotsLeft > 0) availableUpgrades.Add(data);
                else if (data is PassiveData && passiveSlotsLeft > 0) availableUpgrades.Add(data);
            }
        }

        // Iterate through each slot in the upgrade UI and populate the options.
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            // If there are no more available upgrades, then we abort.
            if (availableUpgrades.Count <= 0) return;

            // Pick an upgrade, then remove it so that we don't get it twice.
            YokaiData chosenUpgrade = availableUpgrades[UnityEngine.Random.Range(0, availableUpgrades.Count)];
            availableUpgrades.Remove(chosenUpgrade);

            // Ensure that the selected weapon data is valid.
            if (chosenUpgrade != null)
            {
                // Turns on the UI slot.
                EnableUpgradeUI(upgradeOption);

                // If our inventory already has the upgrade, we will make it a level up.
                Yokai yokai = Get(chosenUpgrade);
                if (yokai)
                {
                    upgradeOption.upgradeButton.onClick.AddListener(() => LevelUp(yokai)); //Apply button functionality
                    if (yokai is Weapon)
                    {
                        Weapon.Stats nextLevel = ((PlayerWeaponData)chosenUpgrade).GetLevelData(yokai.currentLevel + 1);
                        upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                        upgradeOption.upgradeNameDisplay.text = chosenUpgrade.name + " - " + nextLevel.name;
                        upgradeOption.upgradeIcon.sprite = chosenUpgrade.icon;
                    }
                    else
                    {
                        Passive.Modifier nextLevel = ((PassiveData)chosenUpgrade).GetLevelData(yokai.currentLevel + 1);
                        upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                        upgradeOption.upgradeNameDisplay.text = chosenUpgrade.name + " - " + nextLevel.name;
                        upgradeOption.upgradeIcon.sprite = chosenUpgrade.icon;
                    }
                }
                //else
                //{
                //    if (chosenUpgrade is PlayerWeaponData)
                //    {
                //        PlayerWeaponData data = chosenUpgrade as PlayerWeaponData;
                //        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenUpgrade)); //Apply button functionality
                //        upgradeOption.upgradeDescriptionDisplay.text = data.baseStats.description;  //Apply initial description
                //        upgradeOption.upgradeNameDisplay.text = data.baseStats.name;    //Apply initial name
                //        upgradeOption.upgradeIcon.sprite = data.icon;
                //    }
                //    else
                //    {
                //        PassiveData data = chosenUpgrade as PassiveData;
                //        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenUpgrade)); //Apply button functionality
                //        upgradeOption.upgradeDescriptionDisplay.text = data.baseStats.description;  //Apply initial description
                //        upgradeOption.upgradeNameDisplay.text = data.baseStats.name;    //Apply initial name
                //        upgradeOption.upgradeIcon.sprite = data.icon;
                //    }

                //}
            }
        }
    }

    void RemoveUpgradeOptions()
    {
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption);    //Call the DisableUpgradeUI method here to disable all UI options before applying upgrades to them
        }
    }

    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }

    void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }




}
