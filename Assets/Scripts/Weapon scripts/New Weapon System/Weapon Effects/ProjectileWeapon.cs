using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    protected float currentAttackInterval;
    protected int currentAttackCount;

    protected override void Update()
    {
        base.Update();
        if(currentAttackInterval > 0)
        {
            currentAttackInterval -= Time.deltaTime;
            if(currentAttackInterval <= 0) Attack(currentAttackCount);
        }
    }

    public override bool CanAttack()
    {
        if (currentAttackCount > 0) return true;
        return base.CanAttack();
    }

    public override bool Attack(int attackCount = 1)
    {
        if (!currentStats.projectilePrefab)
        {
            currentCoolDown = data.baseStats.cooldown;
            return false;
        }
        if(!CanAttack()) return false;

        float spawnAngle = GetSpawnAngle();

        Projectile prefab = Instantiate(currentStats.projectilePrefab, owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle),
            Quaternion.Euler(0, 0, spawnAngle)); // ������� ������

        prefab.weapon = this;
        prefab.owner = owner;

        if (currentCoolDown <= 0) currentCoolDown += currentStats.cooldown;

        attackCount--;

        if(attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }
        return true;
    }

    private Vector2 shootDirection;
    protected virtual float GetSpawnAngle()
    {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        shootDirection = (worldPosition - (Vector2)transform.position).normalized;
        return Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
    }

    protected virtual Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return Quaternion.Euler(0, 0, spawnAngle) * new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax));
    }
}