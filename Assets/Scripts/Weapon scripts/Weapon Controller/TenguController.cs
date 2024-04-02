using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenguController : WeaponControllers
{
    public Animator animator;
    private Agent parent;
    private PassiveEffect passiveEffect;

    protected override void Start()
    {
        base.Start();
        parent = transform.GetComponentInParent<Agent>();
        passiveEffect = GetComponent<PassiveEffect>();
    }

    protected override void Update()
    {
        base.Update();

    }
    protected override void Attack()
    {
        base.Attack();
        print("TENGU PEW");

        animator.SetTrigger("Attack");

        GameObject spawnedTile = Instantiate(weaponData.prefab);
        spawnedTile.transform.position = transform.position;
        spawnedTile.GetComponent<TenguProjectileBehaviour>().projectileDamage = currentDamage;
        spawnedTile.GetComponent<TenguProjectileBehaviour>().sender = parent.gameObject;
        spawnedTile.GetComponent<TenguProjectileBehaviour>().targetPos = PointerPosition;
        spawnedTile.GetComponent<TenguProjectileBehaviour>().SetDirection();
    }

    public override void LevelUp()
    {
        currentDamage += 1f;
        passiveEffect.multipler += 0.1f;
        passiveEffect.ApplyModifier();
    }

}
