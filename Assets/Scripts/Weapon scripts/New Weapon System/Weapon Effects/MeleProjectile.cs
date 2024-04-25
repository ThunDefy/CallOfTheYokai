using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleProjectile : Projectile
{
    private Transform parentTransform;
    public Vector3 scale;
    Weapon.Stats stats;

    protected override void Start()
    {
        base.Start();

        stats = weapon.GetStats();
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

        // Проверяем, изменился ли масштаб родительского объекта по любой из осей
        if (parentTransform.localScale != new Vector3(1f, 1f, 0f) && alreadyScaled == false)
        {

            // Если да, компенсируем изменения масштаба, чтобы дочерний объект оставался в начальном масштабе
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x) * scale.x, Math.Abs(transform.localScale.x) * scale.y, scale.z);
            alreadyScaled = true;
        }
        else if (parentTransform.localScale == new Vector3(1f, 1f, 0f)) alreadyScaled = false;
    }

    protected List<Health> hitEnemies = new List<Health>(); // Создаем список, чтобы отслеживать уже задетых врагов

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        Health enemy = collider.GetComponent<Health>();
       
        if (enemy && !enemy.isPlayer && !hitEnemies.Contains(enemy)) { 
            hitEnemies.Add(enemy); // Добавляем врага в список задетых
            
            Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;
            enemy.GetHit(GetDamage(), this.gameObject, source, stats.knockback);
        
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }
        StartCoroutine(WaitAndDestroy(stats.lifespan)); // Запускаем корутину для удаления объекта после выполнения анимации
    }

    IEnumerator WaitAndDestroy(float time)
    {
        yield return new WaitForSeconds(time); 
        Destroy(gameObject); 
    }
}
