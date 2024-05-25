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

    float rotationSpeed = 5.0f;
    Quaternion targetRotation;
    Vector2 direction;

    private void FixedUpdate()
    {
        if (GameManager.instance.isGameOver || GameManager.instance.currentState == GameState.Paused
            || GameManager.instance.currentState == GameState.LevelUp)
        {
            return;
        }

        // Вращение оружия вокруг героя

        if (GameManager.instance.controlType == ControlType.Android && isPlayer)
        {
            if(Mathf.Abs(GameManager.instance.shootJoystick.Horizontal) > 0.3f || Mathf.Abs(GameManager.instance.shootJoystick.Vertical) > 0.3f)
            {
                direction = GameManager.instance.shootJoystick.Direction.normalized;
                //print(direction);
                float angle = Mathf.Atan2(GameManager.instance.shootJoystick.Vertical, GameManager.instance.shootJoystick.Horizontal) * Mathf.Rad2Deg;
                targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                RotateWeapon();
            }
            
        }
        else
        {
            direction = (PointerPosition - (Vector2)transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            RotateWeapon();
        }
    }

    private void RotateWeapon()
    {        
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
