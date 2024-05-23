using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Yokai
{
    [System.Serializable]

    public struct Stats
    {
        public string name, description;

        [Header("Visuals")]
        public GameObject prefab;
        public Projectile projectilePrefab;
        public ParticleSystem hitEffect;
        public Rect spawnVariance;

        [Header("Values")]
        public float lifespan;
        public float damage, damageVariance, area, speed, cooldown, projectileInterval, knockback;
        public int number, pircing;

        public static Stats operator +(Stats s1, Stats s2)
        {
            Stats result = new Stats();
            result.name = s2.name ?? s1.name;
            result.description = s2.description ?? s1.description;
            result.prefab = s2.prefab ?? s1.prefab;
            result.projectilePrefab = s2.projectilePrefab ?? s1.projectilePrefab;
            result.hitEffect = s2.hitEffect == null ? s1.hitEffect : s2.hitEffect;
            result.spawnVariance = s2.spawnVariance;
            result.lifespan = s1.lifespan+s2.lifespan;
            result.damage = s1.damage+s2.damage;
            result.damageVariance = s1.damageVariance+s2.damageVariance;
            result.area = s1.area+s2.area;
            result.speed = s1.speed+s2.speed;
            result.cooldown = (s1.cooldown + s2.cooldown) < 0 ? 0 : s1.cooldown + s2.cooldown;
            result.number = s1.number+s2.number;
            result.pircing = s1.pircing+s2.pircing;
            result.projectileInterval = (s1.projectileInterval+s2.projectileInterval) < 0.01f ? 0.01f : s1.projectileInterval + s2.projectileInterval;
            result.knockback = s1.knockback + s2.knockback;
            return result;      
        }

        public float GetDamage()
        {
            return damage + Random.Range(0, damageVariance);
        }
    }

    public Stats currentStats;
    public PlayerWeaponData data;
    public float currentCoolDown;
    //protected Agent player;

    public Animator animator;
    //protected bool attackBlocked;

    PlayerInventory inventory;
    public virtual void Initialise(PlayerWeaponData data)
    {
        print(name + " Initialised");
        base.Initialise(data);
        this.data = data;
        currentStats = data.baseStats;
        //player = GetComponentInParent<Agent>();
        inventory = GetComponentInParent<PlayerInventory>();
        animator = GetComponent<Animator>();

        print("go.transform.rotation " + transform.rotation.z);

    }

    protected virtual void Awake()
    {
        
        if (data)
        {
            Initialise(data);
        }

    }

    protected virtual void Update()
    {
        
        //if (currentCoolDown >= 0)
        //{
        //    currentCoolDown -= Time.deltaTime;
        //}
    }


    public override bool DoLevelUp(int upgradeIndx)
    {
        
        currentStats += data.GetLevelData(upgradeIndx);
        return true;
    }
    public override bool RisingUpYokai() 
    {
        currentRisingLevel++;
        if (!CanLevelUp())
        {
            return false;
        }
        currentStats += data.GetRisingLevelData(currentRisingLevel);
        return true;
    }
    public virtual bool CanAttack()
    {
        return currentCoolDown <= 0;
    }

    public virtual bool Attack(int attackCount = 1)
    {
        if (CanAttack())
        {
            ActivateCoolDown();
            return true;
        }
        return false;
    }

    public virtual float GetDamage()
    {
        return currentStats.GetDamage() * owner.actualStats.power;
    }

    public virtual float GetArea()
    {
        //return currentStats.area * Owner.actualStats.area; 
        float percentageIncrease = Owner.actualStats.area; // Значение процентного увеличения (например, 0.05 - что соответствует 5%)
        float currentArea = currentStats.area * Owner.actualStats.area; 
        float increaseAmount = currentArea * percentageIncrease;
        //return currentArea + increaseAmount;
        return currentStats.area * Owner.actualStats.area;
    }

    public virtual Stats GetStats() { return currentStats; }

    public virtual bool ActivateCoolDown(bool strict = false)
    {
        print("Вызываю куладун");
        if (strict && currentCoolDown > 0) return false;

        float actualCooldown = currentStats.cooldown * (-1*Owner.actualStats.cooldown);

        //currentCoolDown = Mathf.Min(actualCooldown, currentCoolDown + actualCooldown);
        currentCoolDown = currentStats.cooldown + actualCooldown;

        if (inventory!=null) inventory.YokaiActivateColldown(this, currentCoolDown);

        return true;
    }

}
