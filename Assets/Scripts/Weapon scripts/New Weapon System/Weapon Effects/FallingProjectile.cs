using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class FallingProjectile : Projectile
{
    Weapon.Stats stats;
    [HideInInspector]
    public Vector3 targerPosition;

    ParticleSystem zone;
    float area;
    Sounds sounds => GetComponent<Sounds>();

    protected override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        stats = weapon.GetStats();
        area = weapon.GetArea();

        Invoke("CallFalling", 0.8f);

    }


    private void CallFalling()
    {
        if (sounds)
            if (sounds.sounds.Length > 0)
                sounds.PlaySound(0, volume: SoundsController.instance.currentSoundVolume, destroyed: true);
        FallDamageArea();
        InvokeRepeating("BiteDamageArea", 0f, stats.projectileInterval);
        Invoke("DestroyObjectWithDelay", stats.lifespan);
    }

    private void FallDamageArea()
    {

        Collider2D[] targets = Physics2D.OverlapCircleAll(targerPosition, area);
        if (stats.hitEffect)
        {
            zone = Instantiate(stats.hitEffect, transform.position, Quaternion.identity);
            zone.transform.localScale = new Vector3(area * 2, area * 2, 0);
            Destroy(zone, stats.lifespan);
        }

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
        Collider2D[] targets = Physics2D.OverlapCircleAll(targerPosition, area);
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
