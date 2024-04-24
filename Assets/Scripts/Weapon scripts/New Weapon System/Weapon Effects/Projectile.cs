using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : WeaponEffect
{
    public enum DamageSource { projectile, owner};
    public DamageSource damageSource = DamageSource.projectile;
    public bool hasAutoAim = false;
    public Vector3 rotationSpeed = new Vector3(0, 0, 0);
    public Vector3 targetPos;

    
    protected Rigidbody2D rb;
    protected int piercing;

    protected virtual void Start()
    {
        print(transform.localScale);
        rb = GetComponent<Rigidbody2D>();
        
        Weapon.Stats stats = weapon.GetStats();

        if(rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.angularVelocity = rotationSpeed.z;
            rb.velocity = transform.right * stats.speed * weapon.Owner.actualStats.speed;
        }

        float area = weapon.GetArea();

        if (area <= 0) area = 1;
        transform.localScale = new Vector3(area,area, 1);


        //transform.localRotation = Quaternion.Euler(0, 0, -90);

        piercing = stats.pircing;
        if (stats.lifespan > 0) Destroy(gameObject, stats.lifespan);
        if (hasAutoAim) AcquireAutoAimFacing();
        
    }

    public virtual void AcquireAutoAimFacing()
    {
        float aimAngle;
        EnemyAI[] targets = FindObjectsOfType<EnemyAI>();

        if(targets.Length > 0)
        {
            EnemyAI selectedTarget = targets[Random.Range(0, targets.Length)];
            Vector2 difference = selectedTarget.transform.position - transform.position;
            aimAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        }
        else
        {
            aimAngle = Random.Range(0f, 360f);
        }
        transform.rotation = Quaternion.Euler(0, 0, aimAngle - 90);
    }


    protected virtual void FixedUpdate()
    {

        if (rb != null)
        {
            if (rb.bodyType == RigidbodyType2D.Kinematic)
            {
                Weapon.Stats stats = weapon.GetStats();
                transform.position += targetPos * stats.speed * weapon.Owner.actualStats.speed * Time.fixedDeltaTime;
                rb.MovePosition(transform.position);
                transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
            }
        }

    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        Health enemy = collider.GetComponent<Health>();
        if (enemy && !enemy.isPlayer)
        {
            Weapon.Stats stats = weapon.GetStats();
            Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;
            enemy.GetHit(GetDamage(), this.gameObject, source, stats.knockback);

            piercing--;
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }
        if(piercing <0) Destroy(gameObject);
       
    }

}
