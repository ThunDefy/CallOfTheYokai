using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAIOLD : MonoBehaviour
{
    public UnityEvent<Vector2> OnMovementInput, onPointerInput;
    public UnityEvent OnAttack;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private float chaseDistanceThreashold = 3, attackDistanceThreshold = 0.8f;

    [SerializeField]
    private float attckDelay = 1;
    private float passedTime = 1;

    private void Update()
    {
        if (player == null)
        {
            OnMovementInput?.Invoke(Vector2.zero);
            return;
        }
            

        float distance = Vector2.Distance(player.position, transform.position);
        if(distance < chaseDistanceThreashold)
        {
            onPointerInput?.Invoke(player.position);
            if(distance <= attackDistanceThreshold)
            {
                // ������
                print("Rono pew");
                OnMovementInput?.Invoke(Vector2.zero);
                if(passedTime >= attckDelay)
                {
                    passedTime = 0;
                    OnAttack?.Invoke();
                }
            }
            else
            {
                // ��������� ������
                Vector2 direction = player.position - transform.position;
                OnMovementInput?.Invoke(direction.normalized);
            }
        }
        // �����������
        if(passedTime < attckDelay)
        {
            passedTime += Time.deltaTime;
        }
        // ���� ������ ��� �����
        if (distance > chaseDistanceThreashold)
        {
            OnMovementInput?.Invoke(Vector2.zero);
        }
    }
}
