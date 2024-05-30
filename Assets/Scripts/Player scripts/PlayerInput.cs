using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static GameManager;
using TouchPhase = UnityEngine.TouchPhase;

public class PlayerInput : MonoBehaviour
{

    public List<DeadShootTouchZone> deadTouchZone;
    public UnityEvent<Vector2> OnMovementInput, onPointerInput, onTouchInput;
    public UnityEvent OnAttack;
    public UnityEvent OnWeaponSwap;
    public UnityEvent OnDash;



    [SerializeField]
    private InputActionReference movement, Attack, pointerPosition, swapActiveWeapon, dash;

    public ShootControlType cType;

    private void Update()
    {
        OnMovementInput?.Invoke(movement.action.ReadValue<Vector2>());

        if (GameManager.instance.controlType == ControlType.Android)
        {
            if(cType == ShootControlType.Joystick)
            {
                onTouchInput?.Invoke(GetTouchInput());
                if (GameManager.instance.shootJoystick.Horizontal != 0 || GameManager.instance.shootJoystick.Vertical != 0)
                {
                    OnAttack?.Invoke();
                }
            }
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                //for (int i = 0; i < Input.touchCount; i++)
                //{
                List<RaycastResult> results = new List<RaycastResult>();
                PointerEventData eventData = new PointerEventData(EventSystem.current);
                eventData.position = Input.GetTouch(0).position;

                EventSystem.current.RaycastAll(eventData, results);

                if (results.Count > 0)
                {
                    // Получаем информацию о нажатом объекте
                    GameObject tappedObject = results[0].gameObject;
                    if (tappedObject.GetComponent<DeadShootTouchZone>())
                    {
                        Debug.Log("Касание по объекту: " + tappedObject.name);
                        return;
                    } 
                }
                OnAttack?.Invoke();


                //}
            }

        }
        else
        {
            onPointerInput?.Invoke(GetPointerInput());
        }
    }

    // Определение места нахождения курсора
    private Vector2 GetPointerInput()
    {
        if (GameManager.instance.controlType == ControlType.Android)
        {
            //Vector3 stick = new Vector3();
            //stick.z = Mathf.Atan2(GameManager.instance.shootJoystick.Vertical, GameManager.instance.shootJoystick.Horizontal) * Mathf.Rad2Deg;
            //return Camera.main.ScreenToWorldPoint(stick);
            return Vector2.zero;
        }
        else
        {
            Vector3 mousePos = pointerPosition.action.ReadValue<Vector2>();
            mousePos.z = Camera.main.nearClipPlane;
            return Camera.main.ScreenToWorldPoint(mousePos);
        }

    }

    private Vector3 touchPos;
    private Vector2 GetTouchInput()
    {
        if (Input.touchCount > 0) // Проверяем, произошло ли касание
        {
            Touch touch = Input.GetTouch(0); // Получаем информацию о первом касании

            if (touch.phase == TouchPhase.Began) // Проверяем, что касание началось
            {
                touchPos = touch.position; // Получаем позицию касания
                touchPos.z = Camera.main.nearClipPlane;
            }
        }

        return Camera.main.ScreenToWorldPoint(touchPos); // Преобразуем позицию экрана в мировые координаты
    }

    // Атака игрока
    private void OnEnable()
    {
        Attack.action.performed += PerformAttack;
        swapActiveWeapon.action.performed += PerformSwap;
        dash.action.performed += PerformDash;
    }

    private void OnDisable()
    {
        Attack.action.performed -= PerformAttack;
        swapActiveWeapon.action.performed -= PerformSwap;
        dash.action.performed -= PerformDash;
    }

    private void PerformAttack(InputAction.CallbackContext obj)
    {
        OnAttack?.Invoke();
    }

    private void PerformSwap(InputAction.CallbackContext obj)
    {
        OnWeaponSwap?.Invoke();
    }

    private void PerformDash(InputAction.CallbackContext obj)
    {
        OnDash?.Invoke();
    }

}
