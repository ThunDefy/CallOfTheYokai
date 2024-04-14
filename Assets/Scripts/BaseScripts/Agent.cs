using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static GameManager;

public class Agent : MonoBehaviour
{
    public const float DEFUALT_MOVESPEED = 4f;
    
    public float moveSpeed;
    protected Rigidbody2D rb;

    Vector2 knockbackVelocity;
    float knockbackDuration;

    [HideInInspector]
    public Vector2 moveDir;
    private Vector2 movementInput, pointerInput;

    private Weapon playerWeapon;
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
        playerWeapon = GetComponentInChildren<Weapon>();
    }

    private void Update()
    {
        InputManagement();
        weaponParent.PointerPosition = pointerInput;

        if(knockbackDuration > 0)
        {
            Vector3 nextPosition = transform.position + (Vector3)knockbackVelocity * Time.deltaTime;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, knockbackVelocity, out hit, knockbackVelocity.magnitude * Time.deltaTime))
            {
                if (hit.collider.CompareTag("Obstacle"))
                {
                    // Найдено столкновение с коллайдером Obstacle
                    float distanceToObstacle = hit.distance;
                    nextPosition = transform.position + (Vector3)knockbackVelocity.normalized * distanceToObstacle;
                }
            }

            GetComponent<Rigidbody2D>().MovePosition(nextPosition);
            knockbackDuration -= Time.deltaTime;

            //transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            //knockbackDuration -= Time.deltaTime;
        }

    }

    // Передвежение игрока
    void FixedUpdate()
    {
        Move();
    }

    void InputManagement()
    {
        if (GameManager.instance.isGameOver || GameManager.instance.currentState == GameState.Paused
            || GameManager.instance.currentState == GameState.LevelUp)
        {
            return;
        }
        moveDir = movementInput;
    }
    protected virtual void Move()
    {
        if (GameManager.instance.isGameOver || GameManager.instance.currentState == GameState.Paused
            || GameManager.instance.currentState == GameState.LevelUp)
        {
            return;
        }
        rb.velocity = moveDir * DEFUALT_MOVESPEED * moveSpeed;
        //rb.velocity = new Vector2(moveDir.x * moveSpeed, moveDir.y * moveSpeed);
    }

    public void PerformAttack()
    {
        if (GameManager.instance.isGameOver || GameManager.instance.currentState == GameState.Paused
            || GameManager.instance.currentState == GameState.LevelUp)
        {
            return;
        }
        if (weaponController != null)
        {
            weaponController.OnAttack();
            
        }
        else if(playerWeapon==null || !playerWeapon.gameObject.activeSelf)
        {
            playerWeapon = GetComponentInChildren<Weapon>();
            if(playerWeapon) playerWeapon.Attack(playerWeapon.Owner.actualStats.amount);
        }
        else
        {
            playerWeapon.Attack();
        }
    }

    public void Knockback(Vector2 velocity, float duration)
    {
        if (knockbackDuration > 0) return;
        
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }

}
