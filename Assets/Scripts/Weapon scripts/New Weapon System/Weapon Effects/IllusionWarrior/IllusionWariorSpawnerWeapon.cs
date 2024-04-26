using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IllusionWariorSpawnerWeapon : Weapon
{
    private Vector3 targerPosition;
    public override bool Attack(int attackCount = 1)
    {

        if (!currentStats.prefab)
        {
            ActivateCoolDown();
            return false;
        }
        if (!CanAttack())
        {
            return false;
        }

        targerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targerPosition.z = transform.position.z;

        if (animator != null) animator.SetTrigger("Attack");

        GameObject wariorPrefab = Instantiate(currentStats.prefab, targerPosition, Quaternion.identity);
        IllusionWarior warior = wariorPrefab.GetComponent<IllusionWarior>();
        warior.weapon = this;
        warior.owner = owner;

        ActivateCoolDown();

        return true;
    }


   
}
