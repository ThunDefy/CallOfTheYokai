using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<WeaponControllers> weaponSlots = new List<WeaponControllers>(2);
    public List<PassiveEffect> passiveEffects = new List<PassiveEffect>(2);

    public List<Image> weaponUISlots = new List<Image>(2);

    public int[] weaponLevels = new int[2];

    public void AddWeapon(int slotIndex, WeaponControllers weapon, PassiveEffect passiveEffect)
    {
        weaponSlots[slotIndex] = weapon;
        passiveEffects[slotIndex] = passiveEffect;
        weaponLevels[slotIndex] = weapon.weaponData.Level;
        weaponUISlots[slotIndex].enabled = true;
        weaponUISlots[slotIndex].sprite = weapon.weaponData.Icon;

    }

    public void LevelUpWeapon(int slotIndex)
    {
        print(weaponSlots.Count);
        if (weaponSlots.Count > slotIndex)
        {
            //WeaponControllers weapon = weaponSlots[slotIndex];
            //if (weapon != null)
            //{
            //    print(weapon.currentDamage);
            //    print(weapon.currentSpeed);
            //}
            //else
            //{
            //    print("Null");
            //}
            weaponSlots[slotIndex].LevelUp();
        }
    }

}
