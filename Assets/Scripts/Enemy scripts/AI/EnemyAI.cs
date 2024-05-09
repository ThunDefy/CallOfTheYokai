using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private List<SteeringBehaviour> steeringBehaviours;

    [SerializeField]
    private List<Detector> detectors;

    [SerializeField]
    private AIData aiData;

    [SerializeField]
    private float detectionDelay = 0.05f, aiUpdateDelay = 0.1f, attackDelay = 1f;

    [SerializeField]
    private float attackDistance = 0.5f;

    //Inputs sent from the Enemy AI to the Enemy controller
    public UnityEvent OnAttack;
    public UnityEvent<Vector2> OnMovementInput, OnPointerInput;

    [SerializeField]
    private Vector2 movementInput;

    [SerializeField]
    private ContextSolver movementDirectionSolver;

    bool following = false;

    WeaponControllers wc;

    public bool randomWalkAfterAttak = false;
    //public float randomMoveDuration = 0.5f; // Продолжительность случайного движения в с econds
    public float randomMoveRadius = 0.5f;

    private void Start()
    {
        wc = GetComponentInChildren<WeaponControllers>();
        attackDelay = wc.currentColldownDuration;

        //Detecting Player and Obstacles around
        InvokeRepeating("PerformDetection", 0, detectionDelay);

        
    }

    private void PerformDetection()
    {
        foreach (Detector detector in detectors)
        {
            detector.Detect(aiData);
        }

    }

    private void Update()
    {
        //Enemy AI movement based on Target availability
        if (aiData.currentTarget != null)
        {
            //Looking at the Target
            OnPointerInput?.Invoke(aiData.currentTarget.position);
            if (following == false)
            {
                following = true;
                StartCoroutine(ChaseAndAttack());  
            }
            
        }
        else if (aiData.GetTargetsCount() > 0)
        {
            //Target acquisition logic
            aiData.currentTarget = aiData.targets[0];
        }

        //Moving the Agent
        OnMovementInput?.Invoke(movementInput);
    }

    private IEnumerator ChaseAndAttack()
    {
        if (aiData.currentTarget == null)
        {
            //Stopping Logic
            Debug.Log("Stopping");
            movementInput = Vector2.zero;
            following = false;
            yield break;
        }
        else
        {
            float distance = Vector2.Distance(aiData.currentTarget.position, transform.position);

            if (distance < attackDistance)
            {
                ////Random Movement before attack
                //Vector2 randomDirection = Random.insideUnitCircle.normalized * randomMoveRadius;
                //MoveInRandomDirection(randomDirection);

                //Attack logic
                movementInput = Vector2.zero;
                wc.PointerPosition = aiData.currentTarget.position;
                OnAttack?.Invoke();

                if (randomWalkAfterAttak)
                {
                    //Random Movement after attack
                    Vector2 randomDirectionAfterAttack = Random.insideUnitCircle.normalized * randomMoveRadius;
                    StartCoroutine(MoveRandomlyForDuration(randomDirectionAfterAttack, attackDelay));
                }
               

                yield return new WaitForSeconds(attackDelay);

               
                StartCoroutine(ChaseAndAttack());
            }
            else
            {
                //Chase logic
                movementInput = movementDirectionSolver.GetDirectionToMove(steeringBehaviours, aiData);
                yield return new WaitForSeconds(aiUpdateDelay);
                StartCoroutine(ChaseAndAttack());
            }

        }

    }

    private IEnumerator MoveRandomlyForDuration(Vector2 direction, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Двигать врага в указанном направлении
            movementInput = new Vector2(direction.x, direction.y).normalized;
            // Увеличить прошедшее время на время, прошедшее с предыдущего кадра
            elapsedTime += Time.deltaTime;

            // Ждать до следующего кадра
            yield return null;
        }
    }
}
