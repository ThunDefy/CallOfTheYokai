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

    public int currentMapLevel = 0;

    protected virtual void Start()
    {
        currentMapLevel = transform.parent.GetComponent<WeaponParent>().mapLevel;
        Transform parent2 = transform.parent;
        characterRenderer = parent2.parent.GetComponent<SpriteRenderer>();
        
        currentCooldown = weaponData.weaponStats[currentMapLevel].cooldownDuration;
        currentDamage = weaponData.weaponStats[currentMapLevel].damage;
        currentSpeed = weaponData.weaponStats[currentMapLevel].speed;
        print("spawn with " + currentSpeed);

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
        attackBlocked = true;
        currentCooldown = weaponData.weaponStats[currentMapLevel].cooldownDuration;
    }

}
