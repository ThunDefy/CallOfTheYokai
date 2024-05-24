using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class ProjectileWeapon : Weapon
{
    protected float currentAttackInterval;
    protected int currentAttackCount;

    protected Vector2 shootDirection;

    //protected override void Update()
    //{
    //    base.Update();

    //}

    public override bool CanAttack()
    {
        //if (currentAttackCount > 0) return true;
        return base.CanAttack();
    }

    public override bool Attack(int attackCount = 1)
    {

        if (!currentStats.projectilePrefab)
        {
            ActivateCoolDown();
            return false;
        }
        if(!CanAttack()) return false;

        if (animator != null) animator.SetTrigger("Attack");

        float spawnAngle = GetSpawnAngle();
        Projectile prefab = Instantiate(currentStats.projectilePrefab, owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle),Quaternion.Euler(0, 0, spawnAngle - 90)); // создаем снаряд
        prefab.targetPos = shootDirection;
        prefab.weapon = this;
        prefab.owner = owner;

        ActivateCoolDown(true);

        //attackCount--;

        //if (attackCount > 0)
        //{
        //    currentAttackCount = attackCount;
        //    currentAttackInterval = data.baseStats.projectileInterval;
        //}
        return true;
    }

    
    protected virtual float GetSpawnAngle()
    {
        //float agle = prefab.transform.rotation.eulerAngles.z;
        if(GameManager.instance.controlType == ControlType.Android)
        {

            shootDirection = new Vector2(GameManager.instance.moveJoystick.Horizontal, GameManager.instance.moveJoystick.Vertical);
            return Mathf.Atan2(GameManager.instance.moveJoystick.Vertical, GameManager.instance.moveJoystick.Horizontal) * Mathf.Rad2Deg;
            
        }
        else
        {
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            shootDirection = (worldPosition - (Vector2)transform.position).normalized;
            return Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg; 
        }
       
    }

    protected virtual Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        var res = Quaternion.Euler(0, 0, spawnAngle) * new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax));
        print(res);
        return res;
    }
}
