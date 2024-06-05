using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class LightningWeapon : ProjectileWeapon
{
    private Vector3 targerPosition;
    public ParticleSystem zoneVfx;

    Sounds sounds => GetComponent<Sounds>();
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

        float area = GetArea();

        if (GameManager.instance.controlType == ControlType.Android)
        {
            targerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targerPosition.z = transform.position.z;

        }
        else
        {
            targerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targerPosition.z = transform.position.z;
        }

        if (animator != null) animator.SetTrigger("Attack");

        DamageArea(targerPosition, area, GetDamage());
        Instantiate(currentStats.hitEffect, targerPosition, Quaternion.identity);

        if (sounds)
            if (sounds.sounds.Length > 0)
            {
                sounds.PlaySound(0, volume: SoundsController.instance.currentSoundVolume,p1:1f, p2:2f);
                sounds.isPlaying = false;
            }
                

        ActivateCoolDown(true);


        return true;
    }

    private void DamageArea(Vector2 targerPosition, float radius, float damage)
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(targerPosition, radius);

        if (currentStats.prefab)
        {
            zoneVfx = Instantiate(currentStats.prefab.GetComponent<ParticleSystem>(), targerPosition, Quaternion.identity);
            zoneVfx.transform.localScale = new Vector3(radius * 2, radius * 2, 0);
            Destroy(zoneVfx, 0.2f);
        }

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
