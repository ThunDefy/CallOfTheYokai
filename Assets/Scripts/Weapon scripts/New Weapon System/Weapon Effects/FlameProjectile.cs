using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameProjectile : WeaponEffect
{
    public enum DamageSource { projectile, owner };
    public DamageSource damageSource = DamageSource.projectile;
    public Vector3 rotationSpeed = new Vector3(0, 0, 0);
    public Vector3 targetPos;
    private Weapon.Stats stats;
    private Vector3 source;

    protected Rigidbody2D rb;

    protected virtual void Start()
    {
        source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;
        rb = GetComponent<Rigidbody2D>();

        stats = weapon.GetStats();

        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.angularVelocity = rotationSpeed.z;
            rb.velocity = transform.right * stats.speed * weapon.Owner.actualStats.speed;
        }

        float area = weapon.GetArea();
        if (area <= 0) area = 1;
        transform.localScale = new Vector3(area * transform.localScale.x, area * transform.localScale.y, 1);

        Invoke("DestroyObjectWithDelay", stats.lifespan);

    }

    protected virtual void FixedUpdate()
    {
        transform.position = weapon.transform.position;
        transform.rotation = weapon.transform.rotation;

    }

    private float timer = 0f;
    protected virtual void OnTriggerStay2D(Collider2D collider)
    {
        Health enemy = collider.GetComponent<Health>();
        if (enemy && !enemy.isPlayer)
        {
            timer += Time.deltaTime;
            if (timer >= stats.projectileInterval)
            {
                enemy.GetHit(GetDamage(), this.gameObject, source, stats.knockback);
                timer = 0f;
            }

            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }

    }

    void DestroyObjectWithDelay()
    {
        CancelInvoke();
        ((FlamethrowerWeapon)weapon).isAttacking = false;
        weapon.ActivateCoolDown(true);
        Destroy(gameObject);
    }


}
