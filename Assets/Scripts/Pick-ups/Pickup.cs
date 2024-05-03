using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class Pickup : MonoBehaviour
{
    private SpriteAnimation anim;
    protected PlayerStats target;
    protected float speed;
    Vector2 initialPosition;
    float initialOffset;

    [System.Serializable]
    public struct BobbingAnimation
    {
        public float frequency; // �������� ��������
        public Vector2 direction; // �����������

    }
    public BobbingAnimation bobbingAnimation = new BobbingAnimation
    {
        frequency = 2f,
        direction = new Vector2(0, 0.3f)
    };

    [Header("Bonuses")]
    public bool isRareSoul = false;
    public int experience;
    public int health;

    private void Awake()
    {
        anim = GetComponent<SpriteAnimation>();
        //rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        initialPosition = transform.position;
        initialOffset = Random.Range(0, bobbingAnimation.frequency);
    }

    protected virtual void Update()
    {
        if (target)
        {
            // ��������� ������ � ������
            Vector2 distance = target.transform.position - transform.position;
            if (distance.sqrMagnitude > speed * speed * Time.deltaTime)
                transform.position += (Vector3)distance.normalized * speed * Time.deltaTime;
            //else
                
        }
        else
        {
            transform.position = initialPosition + bobbingAnimation.direction * Mathf.Sin((Time.time + initialOffset) * bobbingAnimation.frequency);
        }
    }

    public virtual bool Collect (PlayerStats target, float speed, float lifespan = 0f)
    {

        if (!this.target)
        {
            this.target = target;
            this.speed = speed;
            anim.EndingAnimation();
            return true;
        }
        return false;
    }

    protected virtual void OnDestroy()
    {
        if (!target) return;
        if (experience != 0) target.IncreaseExperience(experience, isRareSoul);
        if(health!=0) target.RestoreHealth(health);
    }
}
