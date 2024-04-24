using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairHookWeapon : VFXProjectileWeapon
{
    public override bool Attack(int attackCount = 1)
    {

        if (!currentStats.projectilePrefab)
        {
            ActivateCoolDown();
            return false;
        }
        if (!CanAttack()) return false;

        if (animator != null) animator.SetTrigger("Attack");

        float spawnAngle = GetSpawnAngle();

        Projectile prefab = Instantiate(currentStats.projectilePrefab, this.transform.position + (Vector3)GetSpawnOffset(spawnAngle), Quaternion.Euler(0, 0, spawnAngle)); // создаем снаряд
        prefab.targetPos = shootDirection;
        prefab.weapon = this;
        prefab.owner = owner;

        RotateVFX(prefab);


        ActivateCoolDown(true);

        //if (attackCount > 0)
        //{
        //    currentAttackCount = attackCount;
        //    currentAttackInterval = data.baseStats.projectileInterval;
        //}

        return true;
    }

}

