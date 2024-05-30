using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static GameManager;
using Unity.Burst;

public class Agent : MonoBehaviour
{
    public const float DEFUALT_MOVESPEED = 4f;

    public bool isPlayer = false;
    public float moveSpeed;
    float originalSpeed;
    protected Rigidbody2D rb;

    Vector2 knockbackVelocity;
    float knockbackDuration;

    public Sounds sounds;
    [Header("Dash Settings")]
    public float dashSpeed =10f;
    public float dashDuration = 1f;
    public float dashCooldown = 1f;


    [HideInInspector]
    public Vector2 moveDir;
    private Vector2 movementInput, pointerInput;

    private Weapon playerWeapon;
    private WeaponControllers weaponController;
    private WeaponParent weaponParent;
    public UnityEvent OnAnimationEventTriggered, OnAttackPeformed;

    public Vector2 MovementInput { get => movementInput; set => movementInput = value; }
    public Vector2 PointerInput { get => pointerInput; set => pointerInput = value; }

    private bool moveIsBlock = false;
    private bool isDashing = false;
    private bool canDash = true;

    private PlayerStats playerStats;

    protected virtual void Awake()
    {
        originalSpeed = moveSpeed;
        weaponParent = GetComponentInChildren<WeaponParent>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        canDash = true;
        weaponController = GetComponentInChildren<WeaponControllers>();
        playerWeapon = GetComponentInChildren<Weapon>();
        playerStats = GetComponent<PlayerStats>();
    }
    private void Update()
    {
        
        weaponParent.PointerPosition = pointerInput;

        if(knockbackDuration > 0)
        {
            Vector3 nextPosition = transform.position + (Vector3)knockbackVelocity * Time.deltaTime;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, knockbackVelocity, out hit, knockbackVelocity.magnitude * Time.deltaTime))
            {
                if (hit.collider.CompareTag("Obstacle"))
                {
                    // ������� ������������ � ����������� Obstacle
                    float distanceToObstacle = hit.distance;
                    nextPosition = transform.position + (Vector3)knockbackVelocity.normalized * distanceToObstacle;
                }
            }

            GetComponent<Rigidbody2D>().MovePosition(nextPosition);
            knockbackDuration -= Time.deltaTime;
        }
    }

    // ������������ 
    void FixedUpdate()
    {
        if (isDashing) return;
        InputManagement();
        Move();
    }
    void InputManagement()
    {
        if (moveIsBlock || GameManager.instance.currentState != GameState.Gameplay)
        {
            return;
        }
        else
        {
            if (GameManager.instance.controlType == ControlType.Android && isPlayer)
            {
                moveDir = new Vector2(GameManager.instance.moveJoystick.Horizontal, GameManager.instance.moveJoystick.Vertical);
            }
            else moveDir = movementInput;

        }
    }
    protected virtual void Move()
    {
        if (moveIsBlock || GameManager.instance.currentState != GameState.Gameplay)
        {
            return;
        }
        else
        {
            rb.velocity = moveDir * DEFUALT_MOVESPEED * moveSpeed;
            if(moveDir != Vector2.zero)
                if (sounds) sounds.PlaySound(0,volume:SoundsController.instance.currentSoundVolume, destroyed:true,p1:0.8f,p2:2.5f);

        }

        }
        public void DoDash()
    {
        if (moveIsBlock || GameManager.instance.currentState != GameState.Gameplay)
        {
            return;
        }
        if (canDash)
        {
            StartCoroutine(Dash());
        }
    }
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        playerStats.canTakeDamage = false;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true); // ������������ �������� ������ � ������������ �����

        float timePassed = 0f;
        Vector2 initialVelocity = rb.velocity;

        while (timePassed < dashDuration)
        {
            float t = timePassed / dashDuration;
            rb.velocity = Vector2.Lerp(initialVelocity, new Vector2(moveDir.x * dashSpeed, moveDir.y * dashSpeed), t);
            timePassed += Time.fixedDeltaTime; // ����������� ������������� ���������� �������

            yield return new WaitForFixedUpdate(); // ��� �������������� ����������
        }

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
        playerStats.canTakeDamage = true;
        rb.velocity = Vector2.zero; // �������� �������� �� ���������� �����
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void PerformAttack()
    {
        if (GameManager.instance.currentState != GameState.Gameplay)
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
            if(playerWeapon) playerWeapon.Attack(playerWeapon.currentStats.number);
        }
        else
        {
            playerWeapon.Attack(playerWeapon.currentStats.number);
        }
    }

    public void Knockback(Vector2 velocity, float duration)
    {
        if (knockbackDuration > 0) return;
        
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }

    public void BlockMovement(bool freez)
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = freez;
        moveIsBlock = freez;
    }
    
    internal void SlowedDown(float by)
    {
        if(by >0)
            moveSpeed *= by;
        else
            moveSpeed = originalSpeed;
    }
}
