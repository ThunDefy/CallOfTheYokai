using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerWeapon : VFXProjectileWeapon
{
    public bool isAttacking = false;
    public override bool Attack(int attackCount = 1)
    {
        if (isAttacking) return false;

        if (!currentStats.prefab)
        {
            ActivateCoolDown();
            return false;
        }

        if (!CanAttack()) return false;   

        if (animator != null) animator.SetTrigger("Attack");

        float spawnAngle = GetSpawnAngle();

        GameObject prefab = Instantiate(currentStats.prefab, owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle), Quaternion.Euler(0, 0, spawnAngle)); // создаем снаряд
        FlameProjectile flame = prefab.GetComponent<FlameProjectile>();
        flame.targetPos = shootDirection;
        flame.weapon = this;
        flame.owner = owner;
        if (data.baseStats.speed == 0) prefab.transform.SetParent(transform);

        isAttacking = true;

        RotateVFX(flame);

        return true;
    }
}
