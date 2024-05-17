using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeaponBehaviour : MonoBehaviour
{
    public WeaponControllers weaponData;

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
        //weaponData = GetComponentInParent<WeaponControllers>();
        Destroy(gameObject, destroyAfterSeconds);
    }


    public void SetDirection()
    {
        if (targetPos != null)
        {
            shootDirection = (targetPos - (Vector2)transform.position).normalized;
        }

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

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

        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, radius))
        {
            Debug.Log(collider.name);
            PlayerStats player;
            if (collider.tag == "Player" && (player = collider.GetComponent<PlayerStats>()))
            {
                //print("POPAAL");
                player.TakeDamage(projectileDamage, gameObject, transform.position);
                Destroy(gameObject);
            }
            else if (collider.tag == "Obstacle")
            {
                Destroy(gameObject);
            }

        }
    }
}
