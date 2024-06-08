using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderWepProjectile : Projectile
{
    Weapon.Stats stats;
    [HideInInspector]
    public Vector3 targerPosition;

    Sounds sounds => GetComponent<Sounds>();


    protected override void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        stats = weapon.GetStats();

        float area = weapon.GetArea();
        if (area <= 0) area = 1;
        transform.localScale = new Vector3(area, area, 1);

        if (sounds)
            if (sounds.sounds.Length > 0)
                sounds.PlaySound(0, volume: SoundsController.instance.currentSoundVolume-0.15f);

        InvokeRepeating("BiteDamage", 0f, stats.projectileInterval);
        Invoke("DestroyObjectWithDelay", stats.lifespan);

    }

    private List<Collider2D> collidedEnemies = new List<Collider2D>();

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            collidedEnemies.Add(other);
            other.GetComponent<Agent>().SlowedDown(0.2f); // задать на сколько замедляет
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            collidedEnemies.Remove(other);
            other.GetComponent<Agent>().SlowedDown(0);
        }
    }


    private void BiteDamage()
    {
        foreach (var enemy in collidedEnemies)
        {
            Health eh = enemy.GetComponent<Health>();
            if (eh)
            {
                if (!eh.isPlayer)
                {
                    eh.GetHit(GetDamage(), this.gameObject, transform.position, stats.knockback);
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
