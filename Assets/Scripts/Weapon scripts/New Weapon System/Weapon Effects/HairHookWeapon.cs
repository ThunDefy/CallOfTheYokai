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

        Projectile prefab = Instantiate(currentStats.projectilePrefab, owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle), Quaternion.Euler(0, 0, spawnAngle)); // создаем снаряд
        prefab.targetPos = shootDirection;
        prefab.weapon = this;
        prefab.owner = owner;
        if (data.baseStats.speed == 0) prefab.transform.SetParent(transform);

        // Поворачиваем партикл по направлению мыши 
        ps = prefab.transform.GetComponent<ParticleSystem>();
        var main = ps.main;
        float agle = prefab.transform.rotation.eulerAngles.z;
        float angleInRadians = agle * Mathf.Deg2Rad;
        angleInRadians = Mathf.Repeat(angleInRadians, Mathf.PI * 2);
        main.startRotation = angleInRadians;


        ActivateCoolDown(true);

        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }

        return true;
    }

}

