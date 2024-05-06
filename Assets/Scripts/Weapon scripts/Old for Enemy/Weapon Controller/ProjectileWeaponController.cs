using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeaponController : WeaponControllers
{
    public Animator animator;
    private Agent parent;

    protected override void Start()
    {
        base.Start();
        parent = transform.GetComponentInParent<Agent>();
    }

    protected override void Update()
    {
        base.Update();

    }
    protected override void Attack()
    {
        base.Attack();

        if(animator) animator.SetTrigger("Attack");

        GameObject spawnedTile = Instantiate(weaponData.prefab);
        spawnedTile.transform.position = transform.position;
        spawnedTile.GetComponent<TenguProjectileBehaviour>().projectileDamage = currentDamage;
        spawnedTile.GetComponent<TenguProjectileBehaviour>().sender = parent.gameObject;
        spawnedTile.GetComponent<TenguProjectileBehaviour>().targetPos = PointerPosition;
        spawnedTile.GetComponent<TenguProjectileBehaviour>().SetDirection();
    }

}
