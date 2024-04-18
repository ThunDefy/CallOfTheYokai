using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class HookProjectile : Projectile
{
    private TrailRenderer trail;
    bool isGrubTurget = false;
    Health grabbedEnemy;
    Weapon.Stats stats;
    Vector3 source;
    Agent enemyAgent;

    protected override void Start()
    {
        trail = GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody2D>();
        
        Weapon.Stats stats = weapon.GetStats();

        float area = weapon.GetArea();
        if (area <= 0) area = 1;
        transform.localScale = new Vector3(area * transform.localScale.x,area * transform.localScale.y, 1);
        trail.startWidth *= weapon.GetArea();

        if (stats.lifespan > 0) Invoke("DestroyObjectWithDelay", stats.lifespan);

    }

    protected override void FixedUpdate()
    {
        if(rb.bodyType == RigidbodyType2D.Kinematic && !isGrubTurget)
        {
            Weapon.Stats stats = weapon.GetStats();
            transform.position += targetPos * stats.speed * weapon.Owner.actualStats.speed* Time.fixedDeltaTime;
            rb.MovePosition(transform.position);
            transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isGrubTurget)
        {
            Health target = collider.GetComponent<Health>();
            if (target && !target.isPlayer)
            {
                grabbedEnemy = target;
                enemyAgent = target.GetComponent<Agent>();
                if (enemyAgent != null) enemyAgent.BlockMovement(true);
                stats = weapon.GetStats();
                source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;
                isGrubTurget = true;
                CancelInvoke("DestroyObjectWithDelay");
                Invoke("DestroyObjectWithDelay", stats.lifespan);
                InvokeRepeating("CallGetHit", 0f, stats.projectileInterval);
            }
        }
    }

    void CallGetHit()
    {
        if(grabbedEnemy!=null)
            grabbedEnemy.GetHit(GetDamage(), this.gameObject, source, stats.knockback);
        else
        {
            CancelInvoke();
            Destroy(gameObject);  
        }
            
    }

    void DestroyObjectWithDelay()
    {
        if(enemyAgent!=null) enemyAgent.BlockMovement(false);
        CancelInvoke();
        Destroy(gameObject);
    }

}
