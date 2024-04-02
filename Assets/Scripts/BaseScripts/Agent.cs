using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static GameManager;

public class Agent : MonoBehaviour
{
    
    public float moveSpeed;
    protected Rigidbody2D rb;
    

    [HideInInspector]
    public Vector2 moveDir;
    private Vector2 movementInput, pointerInput;

    private WeaponControllers weaponController;
    private WeaponParent weaponParent;
    public UnityEvent OnAnimationEventTriggered, OnAttackPeformed;

    public Vector2 MovementInput { get => movementInput; set => movementInput = value; }
    public Vector2 PointerInput { get => pointerInput; set => pointerInput = value; }

    protected virtual void Awake()
    {
        
        weaponParent = GetComponentInChildren<WeaponParent>();
        rb = GetComponent<Rigidbody2D>();
        

    }
    private void Start()
    {
        weaponController = GetComponentInChildren<WeaponControllers>();
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
        if (GameManager.instance.isGameOver || GameManager.instance.currentState == GameState.Paused)
        {
            return;
        }
        moveDir = movementInput;
    }
    protected virtual void Move()
    {
        if (GameManager.instance.isGameOver || GameManager.instance.currentState == GameState.Paused)
        {
            return;
        }
        rb.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
    }

    public void PerformAttack()
    {
        if (GameManager.instance.isGameOver || GameManager.instance.currentState == GameState.Paused)
        {
            return;
        }
        weaponController.OnAttack();
    }
}
