using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class WeaponParent : MonoBehaviour
{
    public Vector2 PointerPosition { get; set; }
    public int mapLevel = 0;
    public bool isPlayer = false;


private void Update()
    {
        if (GameManager.instance.isGameOver || GameManager.instance.currentState == GameState.Paused
            || GameManager.instance.currentState == GameState.LevelUp)
        {
            return;
        }

        // �������� ������ ������ �����

        float rotationSpeed = 5.0f;
        Quaternion targetRotation;
        Vector2 direction;
        if (GameManager.instance.controlType == ControlType.Android && isPlayer)
        {
            direction = new Vector2(GameManager.instance.moveJoystick.Horizontal, GameManager.instance.moveJoystick.Vertical);
            float angle = Mathf.Atan2(GameManager.instance.shootJoystick.Vertical, GameManager.instance.shootJoystick.Horizontal) * Mathf.Rad2Deg;
            targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            // �� �������������� �� ������
           
        }
        else
        {
            direction = (PointerPosition - (Vector2)transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        Vector2 scale = transform.localScale;
        if (direction.x < 0)
        {
            scale.y = -1 * Math.Abs(scale.y);
        }
        else if (direction.x > 0)
        {
            scale.y = Math.Abs(scale.y);
        }
        transform.localScale = scale;

    }

}
