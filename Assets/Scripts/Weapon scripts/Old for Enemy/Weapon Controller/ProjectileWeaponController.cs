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
        TenguProjectileBehaviour spawnedTileBehaviour = spawnedTile.GetComponent<TenguProjectileBehaviour>();
        spawnedTileBehaviour.weaponData = GetComponent<WeaponControllers>();
        spawnedTileBehaviour.transform.position = transform.position;
        spawnedTileBehaviour.projectileDamage = currentDamage;
        spawnedTileBehaviour.sender = parent.gameObject;
        spawnedTileBehaviour.targetPos = PointerPosition;
        spawnedTileBehaviour.SetDirection();
    }

}
