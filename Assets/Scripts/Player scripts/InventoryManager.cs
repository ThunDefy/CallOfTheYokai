using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<WeaponControllers> weaponSlots = new List<WeaponControllers>(4);
    public List<PassiveEffect> passiveEffects = new List<PassiveEffect>(4);
    public List<Image> weaponUISlots = new List<Image>(4);
    public int[] weaponLevels = new int[4];

    //[System.Serializable]
    //public class WeaponUpgrade
    //{
    //    public GameObject initialWeapon;
    //    public WeaponScriptableObject weaponData;

    //}
    //[System.Serializable]
    //public class WeaponPassiveEffectUpgrade
    //{
    //    public GameObject initialPassiveEffect;
    //    public PassiveEffectScriptableObject passiveEffectData;

    //}
    [System.Serializable]
    public class UpgradeUI
    {
        //public TMP_Text upgradeNameDisplay;
        //public TMP_Text upgradeDescriptionDisplay;
        public GameObject upgradeWeaponPanel;
        public Image upgradeIcon;
        public Button upgradeWeaponStatsButton;
        public Button upgradePassivEffectButton;
    }
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>(4);

    //public List<WeaponUpgrade> weaponUpgradesOptions = new List<WeaponUpgrade>();
    //public List<WeaponPassiveEffectUpgrade> weaponPassiveEffectUpgradeOptions = new List<WeaponPassiveEffectUpgrade>();


    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();
    }


    public void AddWeapon(int slotIndex, WeaponControllers weapon, PassiveEffect passiveEffect)
    {
        weaponSlots[slotIndex] = weapon;
        passiveEffects[slotIndex] = passiveEffect;
        //weaponLevels[slotIndex] = weapon.weaponData.Level;
        weaponUISlots[slotIndex].enabled = true;
        weaponUISlots[slotIndex].sprite = weapon.weaponData.Icon;

    }

    public void LevelUpWeapon(int slotIndex)
    {
        print("Power "+slotIndex);
        if (weaponSlots.Count > slotIndex)
        {
            weaponSlots[slotIndex].LevelUp();
        }

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void LevelUpPassiveEffect(int slotIndex)
    {
        print("Passive " + slotIndex);
        if (weaponSlots.Count > slotIndex)
        {
            weaponSlots[slotIndex].LevelUpPassiveEffect();
        }

        if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    void ApplyUpgradeOptionss()
    {
        for (int i = 0; i < weaponSlots.Count; i++)
        {
            int currentIndex = i;
            if (weaponSlots[i] != null)
            {
                upgradeUIOptions[currentIndex].upgradeWeaponPanel.SetActive(true);
                upgradeUIOptions[currentIndex].upgradeWeaponStatsButton.onClick.RemoveAllListeners();
                upgradeUIOptions[currentIndex].upgradePassivEffectButton.onClick.RemoveAllListeners();
                upgradeUIOptions[currentIndex].upgradeWeaponStatsButton.onClick.AddListener(() => LevelUpWeapon(currentIndex));
                upgradeUIOptions[currentIndex].upgradePassivEffectButton.onClick.AddListener(() => LevelUpPassiveEffect(currentIndex));
            }
            else
            {
                upgradeUIOptions[currentIndex].upgradeWeaponPanel.SetActive(false);
            }
        }
    }

    //for (int i = 0; i<weaponSlots.Count; i++)
    //        {
    //            upgradeOption.upgradeWeaponStatsButton.onClick.AddListener(() => LevelUpWeapon(i));
    //            upgradeOption.upgradePassivEffectButton.onClick.AddListener(() => LevelUpPassiveEffect(i));

    //        }


    //void ApplyUpgradeOptions()
    //{
    //    foreach (var upgradeOption in upgradeUIOptions)
    //    {
    //        int upgradeType = Random.Range(1, 3);
    //        if (upgradeType == 1)
    //        {
    //            WeaponUpgrade chosenWeaponUpgrade = weaponUpgradesOptions[Random.Range(0, weaponUpgradesOptions.Count)];

    //            if (chosenWeaponUpgrade != null)
    //            {
    //                bool newWeapon = false;
    //                for (int i = 0; i < weaponSlots.Count; i++)
    //                {
    //                    if (weaponSlots[i] != null && weaponSlots[i].weaponData == chosenWeaponUpgrade.weaponData)
    //                    {
    //                        newWeapon = false;
    //                        if (!newWeapon)
    //                        {
    //                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i));
    //                            upgradeOption.upgradeDescriptionDisplay.text =
    //                                chosenWeaponUpgrade.weaponData.GetComponent<WeaponControllers>().weaponData.weaponDescription;
    //                            upgradeOption.upgradeNameDisplay.text =
    //                                chosenWeaponUpgrade.weaponData.GetComponent<WeaponControllers>().weaponData.weaponName;
    //                        }
    //                        break;
    //                    }
    //                    else
    //                    {
    //                        newWeapon = true;
    //                    }

    //                }
    //                if (newWeapon)
    //                {
    //                    upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnWeapon(chosenWeaponUpgrade.initialWeapon));
    //                }

    //                upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.weaponData.Icon;
    //            }
    //        }
    //        else if (upgradeType == 2)
    //        {
    //            WeaponPassiveEffectUpgrade chosenPassiveEffectUpgrade = weaponPassiveEffectUpgradeOptions[Random.Range(0, weaponPassiveEffectUpgradeOptions.Count)];

    //            if (chosenPassiveEffectUpgrade != null)
    //            {
    //                for (int i = 0; i < passiveEffects.Count; i++)
    //                {
    //                    upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveEffect(i));
    //                }

    //            }
    //        }

    //    }
    //}

}
