using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleProjectile : Projectile
{
    private Transform parentTransform;
    public Vector3 scale;
    protected override void Start()
    {
        base.Start();

        parentTransform = transform.parent.parent.GetComponent<WeaponParent>().transform;
        if (parentTransform.localScale != new Vector3(1f, 1f, 0f))
        {
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x) * scale.x, Math.Abs(transform.localScale.x) * scale.y, scale.z);
        }
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

    protected override void OnTriggerEnter2D(Collider2D collider) // ���� ������ �� ����������, �� ���� ��� �� ������� � piercing
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
        if (piercing < 0) Destroy(gameObject);

    }
}
