using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class FlameProjectile : WeaponEffect
{
    public enum DamageSource { projectile, owner };
    public DamageSource damageSource = DamageSource.projectile;
    public Vector3 rotationSpeed = new Vector3(0, 0, 0);
    public Vector3 targetPos;
    private Weapon.Stats stats;
    private Vector3 source;
    public Animator animator;
    Sounds sounds => GetComponent<Sounds>();

    protected Rigidbody2D rb;

    protected virtual void Start()
    {

        animator.SetBool("CoolDown",false);
        source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;
        rb = GetComponent<Rigidbody2D>();

        stats = weapon.GetStats();

        float area = weapon.GetArea();
        if (area <= 0) area = 1;
        transform.localScale = new Vector3(area, area, 1);

        if (sounds)
            if (sounds.sounds.Length > 0)
                sounds.PlaySound(0, volume: SoundsController.instance.currentSoundVolume);

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
                Destroy(Instantiate(stats.hitEffect, enemy.transform.position, Quaternion.identity), 5f);
            }
        }
    }

    void DestroyObjectWithDelay()
    {
        CancelInvoke();
        ((FlamethrowerWeapon)weapon).isAttacking = false;
        weapon.ActivateCoolDown(true);
        animator.SetBool("CoolDown", true);
        Destroy(gameObject);
    }
}
