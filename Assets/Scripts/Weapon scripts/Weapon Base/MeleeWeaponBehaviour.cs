using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponBehaviour : MonoBehaviour
{
    //public Transform circleOrigin;
    public float radius;
    public bool drawGizmo;
    private void OnDrawGizmosSelected()
    {
        if (drawGizmo) { 
            Gizmos.color = Color.blue;
            //Vector3 position = circleOrigin == null ? Vector3.zero : circleOrigin.position;
            Vector3 position = transform.position;
            Gizmos.DrawWireSphere(position, radius);
        }
    }

    public void DetectColliders(float damage)
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, radius))
        {
            Debug.Log(collider.name);
            Health health;
            if (health = collider.GetComponent<Health>())
            {
                health.GetHit(damage, transform.parent.gameObject);
            }
        }
    }
}
