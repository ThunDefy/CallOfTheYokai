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

    Weapon.Stats wStats;
    Sounds sounds => GetComponent<Sounds>();

    protected virtual void Start()
    {
        //print(transform.localScale);
        rb = GetComponent<Rigidbody2D>();

        wStats = weapon.GetStats();

        if(rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.angularVelocity = rotationSpeed.z;
            rb.velocity = transform.right * wStats.speed * weapon.Owner.actualStats.speed;
        }

        float area = weapon.GetArea();

        if (area <= 0) area = 1;
        transform.localScale = new Vector3(area,area, 1);


        //transform.localRotation = Quaternion.Euler(0, 0, -90);

        piercing = wStats.pircing;
        if (wStats.lifespan > 0) Destroy(gameObject, wStats.lifespan);
        if (hasAutoAim) AcquireAutoAimFacing();

        if (sounds)
            if (sounds.sounds.Length > 0)
                sounds.PlaySound(0, volume: SoundsController.instance.currentSoundVolume, destroyed: true);

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
            Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;
            enemy.GetHit(GetDamage(), this.gameObject, source, wStats.knockback);

            piercing--;
            if (wStats.hitEffect)
            {
                Destroy(Instantiate(wStats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }
        if(piercing <0) Destroy(gameObject);

        if(collider.tag == "Obstacle") Destroy(Instantiate(wStats.hitEffect, transform.position, Quaternion.identity), 5f);

    }

}
