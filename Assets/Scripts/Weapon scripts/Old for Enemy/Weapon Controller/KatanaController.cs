using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatanaController : WeaponControllers
{
    public Animator animator;
    private MeleeWeaponBehaviour hitRadius;


    protected override void Start()
    {
        base.Start();
        hitRadius = transform.GetComponentInChildren<MeleeWeaponBehaviour>();
    }

    protected override void Update()
    {
        base.Update();

    }
    protected override void Attack()
    {
        base.Attack();
        print("Katana WHEEU");
        animator.SetTrigger("Attack");

        hitRadius.DetectColliders(currentDamage, weaponData.target);
    }

    

}
