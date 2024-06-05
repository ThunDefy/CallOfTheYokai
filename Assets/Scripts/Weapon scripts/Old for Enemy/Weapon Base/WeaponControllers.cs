using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    private Sounds sounds => GetComponent<Sounds>();

    protected virtual void Start()
    {
        currentMapLevel = transform.parent.GetComponent<WeaponParent>().mapLevel;
        Transform parent2 = transform.parent;
        characterRenderer = parent2.parent.GetComponent<SpriteRenderer>();

        currentColldownDuration = weaponData.weaponStats[currentMapLevel].cooldownDuration;
        currentCooldown = currentColldownDuration;
        currentDamage = weaponData.weaponStats[currentMapLevel].damage;
        currentSpeed = weaponData.weaponStats[currentMapLevel].speed;
        //print("spawn with " + currentSpeed);

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
            if (sounds)
                if (sounds.sounds.Length > 0)
                    sounds.PlaySound(0, volume: SoundsController.instance.currentSoundVolume);
            Attack();
        }
    }

    protected virtual void Attack()
    {
        attackBlocked = true;
        currentCooldown = currentColldownDuration;
        
    }

}
