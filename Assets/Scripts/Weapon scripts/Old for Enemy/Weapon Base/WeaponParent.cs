using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class WeaponParent : MonoBehaviour
{
    public Vector2 PointerPosition { get; set; }

    private void Update()
    {
        if (GameManager.instance.isGameOver || GameManager.instance.currentState == GameState.Paused
            || GameManager.instance.currentState == GameState.LevelUp)
        {
            return;
        }

        // �������� ������ ������ �����

        float rotationSpeed = 5.0f;
        Vector2 direction = (PointerPosition - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        Vector2 scale = transform.localScale;
        if (direction.x < 0)
        {
            scale.y = -1;
        }
        else if (direction.x > 0)
        {
            scale.y = 1;
        }
        transform.localScale = scale;

        

    }

}
