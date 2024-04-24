using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingWeapon : ProjectileWeapon
{
    private Vector3 targerPosition;

    public override bool Attack(int attackCount = 1)
    {

        if (!currentStats.projectilePrefab)
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


        FallingProjectile fallingPrefab = Instantiate((FallingProjectile)currentStats.projectilePrefab, targerPosition, Quaternion.identity); // вызвать фоллинг прожектайл
        fallingPrefab.targerPosition = targerPosition;
        fallingPrefab.weapon = this;
        fallingPrefab.owner = owner;

        ActivateCoolDown(true);

        
        return true;
    }

}
