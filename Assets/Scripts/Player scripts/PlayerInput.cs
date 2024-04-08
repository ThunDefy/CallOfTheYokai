using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public UnityEvent<Vector2> OnMovementInput, onPointerInput;
    public UnityEvent OnAttack;
    public UnityEvent OnWeaponSwap;

    [SerializeField]
    private InputActionReference movement, Attack, pointerPosition, swapActiveWeapon;

    private void Update()
    {
        OnMovementInput?.Invoke(movement.action.ReadValue<Vector2>());
        onPointerInput?.Invoke(GetPointerInput());
    }

    // Определение места нахождения курсора
    private Vector2 GetPointerInput()
    {
        Vector3 mousePos = pointerPosition.action.ReadValue<Vector2>();
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    // Атака игрока
    private void OnEnable()
    {
        Attack.action.performed += PerformAttack;
        swapActiveWeapon.action.performed += PerformSwap;
    }

    private void OnDisable()
    {
        Attack.action.performed -= PerformAttack;
        swapActiveWeapon.action.performed -= PerformSwap;
    }

    private void PerformAttack(InputAction.CallbackContext obj)
    {
        OnAttack?.Invoke();
    }

    private void PerformSwap(InputAction.CallbackContext obj)
    {
        OnWeaponSwap?.Invoke();
    }

}
