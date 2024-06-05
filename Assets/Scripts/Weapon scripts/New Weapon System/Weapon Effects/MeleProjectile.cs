using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleProjectile : Projectile
{
    private Transform parentTransform;
    public Vector3 scale;
    Weapon.Stats stats;
    Sounds sounds => GetComponent<Sounds>();

    protected override void Start()
    {
        base.Start();

        stats = weapon.GetStats();
        parentTransform = transform.parent.parent.GetComponent<WeaponParent>().transform;
        if (parentTransform.localScale != new Vector3(1f, 1f, 0f))
        {
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x) * scale.x, Math.Abs(transform.localScale.x) * scale.y, scale.z);
        }
        if (sounds)
            if (sounds.sounds.Length > 0)
                sounds.PlaySound(0, volume: SoundsController.instance.currentSoundVolume, destroyed:true);
            
                


    }
    bool alreadyScaled = false;
    protected override void FixedUpdate()
    {

        base.FixedUpdate();

        // ���������, ��������� �� ������� ������������� ������� �� ����� �� ����
        if (parentTransform.localScale != new Vector3(1f, 1f, 0f) && alreadyScaled == false)
        {

            // ���� ��, ������������ ��������� ��������, ����� �������� ������ ��������� � ��������� ��������
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x) * scale.x, Math.Abs(transform.localScale.x) * scale.y, scale.z);
            alreadyScaled = true;
        }
        else if (parentTransform.localScale == new Vector3(1f, 1f, 0f)) alreadyScaled = false;
    }

    protected List<Health> hitEnemies = new List<Health>(); // ������� ������, ����� ����������� ��� ������� ������

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        Health enemy = collider.GetComponent<Health>();
       
        if (enemy && !enemy.isPlayer && !hitEnemies.Contains(enemy)) { 
            hitEnemies.Add(enemy); // ��������� ����� � ������ �������
            
            Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;
            enemy.GetHit(GetDamage(), this.gameObject, source, stats.knockback);
        
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
            if (sounds)
                if (sounds.sounds.Length > 1)
                {
                    sounds.isPlaying = false;
                    sounds.PlaySound(1, volume: SoundsController.instance.currentSoundVolume);
                }
        }
        StartCoroutine(WaitAndDestroy(stats.lifespan)); // ��������� �������� ��� �������� ������� ����� ���������� ��������
    }

    IEnumerator WaitAndDestroy(float time)
    {
        yield return new WaitForSeconds(time); 
        Destroy(gameObject); 
    }
}
