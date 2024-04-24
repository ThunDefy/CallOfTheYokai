using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class FallingProjectile : Projectile
{
    Weapon.Stats stats;
    [HideInInspector]
    public Vector3 targerPosition;

    protected override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        stats = weapon.GetStats();

        Invoke("CallFalling", 0.8f);

    }


    private void CallFalling()
    {
        FallDamageArea();
        InvokeRepeating("BiteDamageArea", 0f, stats.projectileInterval);
        Invoke("DestroyObjectWithDelay", stats.lifespan);
    }

    private void FallDamageArea()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(targerPosition, stats.area);
        foreach (Collider2D t in targets)
        {
            Health eh = t.GetComponent<Health>();
            if (eh)
            {
                if (!eh.isPlayer)
                {
                    eh.GetHit(GetDamage() * 10, this.gameObject, transform.position, stats.knockback);
                }

            }
        }
    }

    private void BiteDamageArea()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(targerPosition, stats.area);
        foreach (Collider2D t in targets)
        {
            Health eh = t.GetComponent<Health>();
            if (eh)
            {
                if (!eh.isPlayer)
                {
                    eh.GetHit(GetDamage(), this.gameObject, transform.position, 0.5f);
                }

            }
        }
    }

    private void DestroyObjectWithDelay()
    {
        CancelInvoke();
        Destroy(gameObject);
    }

}
