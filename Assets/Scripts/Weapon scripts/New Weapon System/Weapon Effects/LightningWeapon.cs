using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningWeapon : ProjectileWeapon
{
    private Vector3 targerPosition;
    public override bool Attack(int attackCount = 1)
    {

        if (!currentStats.hitEffect)
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

        DamageArea(targerPosition, currentStats.area, GetDamage());
        Instantiate(currentStats.hitEffect, targerPosition, Quaternion.identity);

        ActivateCoolDown(true);

        attackCount--;

        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }
        return true;
    }

    private void DamageArea(Vector2 targerPosition, float radius, float damage)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(targerPosition, radius);
        foreach (Collider2D t in targets)
        {
            Health eh = t.GetComponent<Health>();
            if (eh)
            {
                if (!eh.isPlayer)
                {
                    eh.GetHit(damage, this.gameObject, transform.position, currentStats.knockback);
                }
                
            }
        }
    }
}
