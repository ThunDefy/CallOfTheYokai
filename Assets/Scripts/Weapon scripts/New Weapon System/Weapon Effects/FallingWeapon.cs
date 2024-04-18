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

        attackCount--;

        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }
        return true;
    }

    
    //private void CallFalling()
    //{
    //    FallDamageArea();
    //    InvokeRepeating("BiteDamageArea", 0f, currentStats.projectileInterval);
    //    Invoke("DestroyObjectWithDelay", currentStats.lifespan);
    //}

    //private void FallDamageArea()
    //{
    //    Collider2D[] targets = Physics2D.OverlapCircleAll(targerPosition, currentStats.area);
    //    foreach (Collider2D t in targets)
    //    {
    //        Health eh = t.GetComponent<Health>();
    //        if (eh)
    //        {
    //            if (!eh.isPlayer)
    //            {
    //                eh.GetHit(GetDamage() * 10, this.gameObject, transform.position, currentStats.knockback);
    //            }

    //        }
    //    }
    //}

    //private void BiteDamageArea()
    //{
    //    Collider2D[] targets = Physics2D.OverlapCircleAll(targerPosition, currentStats.area);
    //    foreach (Collider2D t in targets)
    //    {
    //        Health eh = t.GetComponent<Health>();
    //        if (eh)
    //        {
    //            if (!eh.isPlayer)
    //            {
    //                eh.GetHit(GetDamage(), this.gameObject, transform.position, currentStats.knockback);
    //            }

    //        }
    //    }
    //}

    //void DestroyObjectWithDelay(int num)
    //{
    //    CancelInvoke();
    //    Destroy(projectiles[num].transform.gameObject);
    //}
}
