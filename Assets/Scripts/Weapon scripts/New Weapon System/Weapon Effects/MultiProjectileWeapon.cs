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

        float area = GetArea();
        if (area <= 0) area = 1;

        while (attackCount > 0)
        {

            Projectile prefab = Instantiate(currentStats.projectilePrefab, this.transform.position + (Vector3)GetSpawnOffset(spawnAngle), Quaternion.Euler(0, 0, spawnAngle)); // ������� ������
            prefab.targetPos = spreads[num++];
            prefab.weapon = this;
            prefab.owner = owner;

            prefab.transform.localScale = new Vector3(area, area, 1);

            RotateVFX(prefab);

            attackCount--;
        }
        ActivateCoolDown(true);

        //if (attackCount <= 0)
        //{
        //    currentAttackCount = attackCount;
        //    currentAttackInterval = data.baseStats.projectileInterval;
        //}
        return true;
    }

    private List<Vector2> GenerateTriangleSpread(int projectileCount)
    {
        List<Vector2> spreads = new List<Vector2>();
        float spreadAngleIncrement = 20f / (projectileCount - 1); // ������ ���� ����� ���������� �� ������������

        for (int i = 0; i < projectileCount; i++)
        {
            float spreadAngle = -10f + i * spreadAngleIncrement; // ���������� ���� ��� ������� ����
            Vector2 spreadDirection = Quaternion.Euler(0, 0, spreadAngle) * shootDirection;

            spreads.Add(spreadDirection);
        }

        return spreads;
    }

    private List<Vector2> GenerateRandomSpread(int projectileCount,float min = -20f, float max = 20f)
    {
        List<Vector2> spreads = new List<Vector2>();
        HashSet<float> usedAngles = new HashSet<float>(); // ������� ��������� ��� ������������ �������������� �����

        for (int i = 0; i < projectileCount; i++)
        {
            float spreadAngle;
            do
            {
                spreadAngle = Random.Range(min, max); // ��������� ����������� ���� ��� ������ ����
            } while (usedAngles.Contains(spreadAngle)); // ���������, ������������� �� ��� ����� ����

            usedAngles.Add(spreadAngle); // ��������� ���� � ������ ��������������
            Vector2 spreadDirection = Quaternion.Euler(0, 0, spreadAngle) * shootDirection;

            spreads.Add(spreadDirection);
        }

        return spreads;
    }

}
