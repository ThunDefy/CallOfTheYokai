using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IllusionWarior : WeaponEffect
{
    // Раздать характеристики из данных о оружии : Agent, Health, KatanaMele, уничтожить как пройдет lifespan

    public enum DamageSource { projectile, owner };
    public DamageSource damageSource = DamageSource.projectile;

    private void Start()
    {
        Weapon.Stats stats = weapon.GetStats();

        float area = weapon.GetArea();
        if (area <= 0) area = 1;
        transform.localScale = new Vector3(area * transform.localScale.x, area * transform.localScale.y, 1);

        if (stats.lifespan > 0)
        {
            Invoke("SelfDestructSequence", stats.lifespan);
            
        }

        KatanaController weaponStat = GetComponentInChildren<KatanaController>();
        weaponStat.currentDamage = GetDamage();
        weaponStat.currentCooldown = stats.projectileInterval;
        weaponStat.currentPierce = stats.pircing;
        weaponStat.currentSpeed = stats.speed;
        GetComponent<Agent>().moveSpeed = stats.speed;  

    }

    void SelfDestructSequence()
    {
        Destroy(gameObject);
    }
}
