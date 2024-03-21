using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Agent : MonoBehaviour
{
    
    public float moveSpeed;
    Rigidbody2D rb;

    [HideInInspector]
    public Vector2 moveDir;
    private Vector2 movementInput, pointerInput;

    private WeaponControllers weaponController;
    private WeaponParent weaponParent;
    public UnityEvent OnAnimationEventTriggered, OnAttackPeformed;

    public Vector2 MovementInput { get => movementInput; set => movementInput = value; }
    public Vector2 PointerInput { get => pointerInput; set => pointerInput = value; }

    private void Awake()
    {
        weaponController = GetComponentInChildren<WeaponControllers>();
        weaponParent = GetComponentInChildren<WeaponParent>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        InputManagement();
        weaponParent.PointerPosition = pointerInput;
        weaponController.PointerPosition = pointerInput;
    }

    // Передвежение игрока
    void FixedUpdate()
    {
        Move();
    }

    void InputManagement()
    {
        moveDir = movementInput;
    }
    void Move()
    {
        rb.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
    }

    public void PerformAttack()
    {
        weaponController.OnAttack();
        
    }




}
