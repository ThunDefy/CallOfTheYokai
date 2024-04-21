using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderWebWeapon : ProjectileWeapon
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

        SpiderWepProjectile spiderWebPrefab = Instantiate((SpiderWepProjectile)currentStats.projectilePrefab, targerPosition, Quaternion.identity); 
        spiderWebPrefab.targerPosition = targerPosition;
        spiderWebPrefab.weapon = this;
        spiderWebPrefab.owner = owner;

        ActivateCoolDown(true);

        attackCount--;

        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }
        return true;
    }

   
}
