using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeaponBehaviour : MonoBehaviour
{
    //public WeaponScriptableObject weaponData;

    protected WeaponControllers weaponData;

    public Transform circleOrigin;
    public float radius;

    public float projectileDamage;
    public GameObject sender;

    public Vector2 targetPos;

    protected Vector3 direction;
    public float destroyAfterSeconds;

    private Vector2 shootDirection;


    protected virtual void Start()
    {
        weaponData = FindAnyObjectByType<WeaponControllers>();
        Destroy(gameObject, destroyAfterSeconds);
    }
    public float GetCurrentDamage()
    {
        return projectileDamage *= FindAnyObjectByType<PlayerStats>().actualStats.power;
    }

    public void SetDirection()
    {
        if (targetPos == null)
        {
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            shootDirection = (worldPosition - (Vector2)transform.position).normalized;
        }
        else
        {
            shootDirection = (targetPos - (Vector2)transform.position).normalized;
        }
       

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle-90);

        direction = shootDirection;

    }

    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 position = circleOrigin == null ? Vector3.zero : circleOrigin.position;
        Gizmos.DrawWireSphere(position, radius);
    }

    public void DetectColliders()
    {
        //foreach (Collider2D collider in Physics2D.OverlapCircleAll(circleOrigin.position, radius))
        //{
        //    Debug.Log(collider.name);
        //    Health health;
        //    if (health = collider.GetComponent<Health>())
        //    {
        //        health.GetHit(GetCurrentDamage(), sender,transform.position);
        //    }
        //    if(collider.name != "Collector") 
        //        Destroy(gameObject);
        //}

        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, radius))
        {
            Debug.Log(collider.name);
            PlayerStats player;
            if (collider.tag == "player" && (player = collider.GetComponent<PlayerStats>()))
            {
                player.TakeDamage(GetCurrentDamage(), transform.parent.gameObject, transform.position);
                Destroy(gameObject);
            }
            else Destroy(gameObject);

        }
    }
}
