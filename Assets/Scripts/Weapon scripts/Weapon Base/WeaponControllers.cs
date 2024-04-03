using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponControllers : MonoBehaviour
{
    public Vector2 PointerPosition { get; set; }
    public SpriteRenderer characterRenderer, weaponRenderer;

    [Header("Weapon Stats")]
    public WeaponScriptableObject weaponData;

    public float currentCooldown;
    public float currentDamage;
    public float currentSpeed;
    public float currentColldownDuration;
    public float currentPierce;

    private bool attackBlocked;

    protected virtual void Start()
    {
        Transform parent2 = transform.parent;
        characterRenderer = parent2.parent.GetComponent<SpriteRenderer>();

        currentCooldown = weaponData.cooldownDuration;
        currentDamage = weaponData.damage;
        currentSpeed = weaponData.speed;
        currentPierce = weaponData.pierce;

        attackBlocked = true;

    }

    protected virtual void Update()
    {

        // Перезарядка атаки

        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0f)
        {
            attackBlocked = false;
        }

        if (transform.eulerAngles.z > 0 && transform.eulerAngles.z < 180)
        {
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder - 1;
        }
        else
        {
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder + 1;
        }

    }
    public void OnAttack()
    {
        if (attackBlocked == false)
        {
            Attack();
        }
    }

    protected virtual void Attack()
    {
        print("Controller PEW");
        attackBlocked = true;
        currentCooldown = weaponData.cooldownDuration;
    }

    public virtual void LevelUp()
    {

    }

    public virtual void LevelUpPassiveEffect()
    {
        throw new NotImplementedException();
    }
}
