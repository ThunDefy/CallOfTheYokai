using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXProjectileWeapon : ProjectileWeapon
{
    private ParticleSystem ps;
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
        Projectile prefab = Instantiate(currentStats.projectilePrefab, owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle), Quaternion.Euler(0, 0, spawnAngle)); // ������� ������
        prefab.targetPos = shootDirection;
        prefab.weapon = this;
        prefab.owner = owner;
        prefab.transform.SetParent(transform);
        // ������������ ������� �� ����������� ���� 
        ps = prefab.transform.GetComponent<ParticleSystem>();
        var main = ps.main;
        float agle = prefab.transform.rotation.eulerAngles.z;
        float angleInRadians = agle * Mathf.Deg2Rad;
        angleInRadians = Mathf.Repeat(angleInRadians, Mathf.PI * 2);
        print(agle);
        main.startRotation = angleInRadians;

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
