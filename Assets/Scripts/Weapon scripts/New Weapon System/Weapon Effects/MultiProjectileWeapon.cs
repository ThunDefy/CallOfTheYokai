using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiProjectileWeapon : VFXProjectileWeapon
{

    public override bool Attack(int attackCount = 1)
    {
        if (!currentStats.projectilePrefab)
        {
            ActivateCoolDown();
            return false;
        }
        if (!CanAttack()) return false;

        if (animator != null) animator.SetTrigger("Attack");

        float spawnAngle = GetSpawnAngle();

        List<Vector2> spreads = GenerateTriangleSpread(attackCount);
        int num = 0;

        while (attackCount > 0)
        {

            Projectile prefab = Instantiate(currentStats.projectilePrefab, owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle), Quaternion.Euler(0, 0, spawnAngle)); // создаем снар€д
            prefab.targetPos = spreads[num++];
            prefab.weapon = this;
            prefab.owner = owner;
            if (data.baseStats.speed == 0) prefab.transform.SetParent(transform);

            // ѕоворачиваем партикл по направлению мыши 
            ps = prefab.transform.GetComponent<ParticleSystem>();
            var main = ps.main;
            float agle = prefab.transform.rotation.eulerAngles.z;
            float angleInRadians = agle * Mathf.Deg2Rad;
            angleInRadians = Mathf.Repeat(angleInRadians, Mathf.PI * 2);
            prefab.transform.rotation = Quaternion.Euler(0, 0, agle);
            main.startRotation = angleInRadians;

            attackCount--;
        }
        ActivateCoolDown(true);

        if (attackCount <= 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }
        return true;
    }

    private List<Vector2> GenerateTriangleSpread(int projectileCount)
    {
        List<Vector2> spreads = new List<Vector2>();
        float spreadAngleIncrement = 20f / (projectileCount - 1); // –асчет угла между разбросами по треугольнику

        for (int i = 0; i < projectileCount; i++)
        {
            float spreadAngle = -10f + i * spreadAngleIncrement; // ¬ычисление угла дл€ текущей пули
            Vector2 spreadDirection = Quaternion.Euler(0, 0, spreadAngle) * shootDirection;

            spreads.Add(spreadDirection);
        }

        return spreads;
    }

    private List<Vector2> GenerateRandomSpread(int projectileCount,float min = -20f, float max = 20f)
    {
        List<Vector2> spreads = new List<Vector2>();
        HashSet<float> usedAngles = new HashSet<float>(); // —оздаем множество дл€ отслеживани€ использованных углов

        for (int i = 0; i < projectileCount; i++)
        {
            float spreadAngle;
            do
            {
                spreadAngle = Random.Range(min, max); // √енераци€ уникального угла дл€ каждой пули
            } while (usedAngles.Contains(spreadAngle)); // ѕровер€ем, использовалс€ ли уже такой угол

            usedAngles.Add(spreadAngle); // ƒобавл€ем угол в список использованных
            Vector2 spreadDirection = Quaternion.Euler(0, 0, spreadAngle) * shootDirection;

            spreads.Add(spreadDirection);
        }

        return spreads;
    }

}
