using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidanceBehaviour : SteeringBehaviour
{
    [SerializeField]
    private float radius = 2f, agentColliderSize = 0.6f;

    [SerializeField]
    private bool showGizmo = true;

    //gizmo parameters
    float[] dangersResultTemp = null;

    public override (float[] danger, float[] interest) GetSteering(float[] danger, float[] interest, AIData aiData)
    {
        foreach (Collider2D obstacleCollider in aiData.obstacles)
        {
            if (obstacleCollider != null)
            {
                Vector2 directionToObstacle
                    = obstacleCollider.ClosestPoint(transform.position) - (Vector2)transform.position;
                float distanceToObstacle = directionToObstacle.magnitude;

                //�������� ���� �� ������ ���������� �� ���� <---> �����������
                float weight
                    = distanceToObstacle <= agentColliderSize
                    ? 1
                    : (radius - distanceToObstacle) / radius;

                Vector2 directionToObstacleNormalized = directionToObstacle.normalized;

                //������� ��������� ����������� � ������ ����������
                for (int i = 0; i < Directions.eightDirections.Count; i++)
                {
                    float result = Vector2.Dot(directionToObstacleNormalized, Directions.eightDirections[i]);
                    // ��������� ������������ 2� �������� �� ����������� � ����������� � ������ �� �����������
                    // ��� �� ����� �� ������� ������ �� � �����������

                    float valueToPutIn = result * weight;

                    //�������������� �������� ������ � ��� ������, ���� ��� ���� ��������, ������������ � ������� ����������
                    if (valueToPutIn > danger[i])
                    {
                        danger[i] = valueToPutIn;
                    }
                }
            }
        }
        dangersResultTemp = danger;
        return (danger, interest);
    }
    private void OnDrawGizmos()
    {
        if (showGizmo == false)
            return;

        if (Application.isPlaying && dangersResultTemp != null)
        {
            if (dangersResultTemp != null)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < dangersResultTemp.Length; i++)
                {
                    Gizmos.DrawRay(
                        transform.position,
                        Directions.eightDirections[i] * dangersResultTemp[i] * 2
                        );
                }
            }
        }
    }
}

public static class Directions
{
    // �������� ����������� ������ �������� ��� 8 �����������
    public static List<Vector2> eightDirections = new List<Vector2>{
            new Vector2(0,1).normalized,
            new Vector2(1,1).normalized,
            new Vector2(1,0).normalized,
            new Vector2(1,-1).normalized,
            new Vector2(0,-1).normalized,
            new Vector2(-1,-1).normalized,
            new Vector2(-1,0).normalized,
            new Vector2(-1,1).normalized
        };
}
