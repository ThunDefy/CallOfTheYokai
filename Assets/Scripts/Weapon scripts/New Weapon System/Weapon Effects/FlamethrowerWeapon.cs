using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerWeapon : VFXProjectileWeapon
{
    public bool isAttacking = false;

    protected override void Update()
    {
        if (currentCoolDown > 0)
        {
            currentCoolDown -= Time.deltaTime;
        }
        if (currentCoolDown <= 0f)
        {
            animator.SetBool("CoolDown", false);
        }
    }

    public override bool Attack(int attackCount = 1)
    {
        if (isAttacking) return false;

        if (!currentStats.prefab)
        {
            ActivateCoolDown();
            return false;
        }

        if (!CanAttack()) return false;   

        if (animator != null) 
            animator.SetTrigger("Attack");

        isAttacking = true;

        StartCoroutine(WaitForAnimation());
       
        return true;
    }

    IEnumerator WaitForAnimation()
    {

        // Подождать, пока анимация не завершится
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("GotokoNekoStartAttack"))
            yield return null;
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("GotokoNekoStartAttack"))
            yield return null;
      
        float spawnAngle = GetSpawnAngle();

        GameObject prefab = Instantiate(currentStats.prefab, owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle), Quaternion.Euler(0, 0, spawnAngle)); // создаем снаряд
        FlameProjectile flame = prefab.GetComponent<FlameProjectile>();
        flame.targetPos = shootDirection;
        flame.weapon = this;
        flame.owner = owner;
        flame.animator = animator;

        prefab.transform.SetParent(transform);

        RotateVFX(flame);
    }

}
