using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponControllers : MonoBehaviour
{
    public Vector2 PointerPosition { get; set; }
    public SpriteRenderer characterRenderer, weaponRenderer;

    [Header("Weapon Stats")]

    public GameObject prefab;
    public float damage;
    public float speed;
    public float cooldownDuration;
    float currentCooldown;
    private bool attackBlocked;
    public int pierce;

    protected virtual void Start()
    {
        currentCooldown = cooldownDuration;
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
        currentCooldown = cooldownDuration;
    }

    


}
