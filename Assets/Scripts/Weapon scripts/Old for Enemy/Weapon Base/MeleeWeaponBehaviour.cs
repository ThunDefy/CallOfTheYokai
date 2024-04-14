using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponBehaviour : MonoBehaviour
{
    //public Transform circleOrigin;
    public WeaponScriptableObject weaponData;
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

    public void DetectColliders(float damage,string target)
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(transform.position, radius))
        {
            Debug.Log(collider.name);
            PlayerStats player;
            if (collider.name == target && (player = collider.GetComponent<PlayerStats>()))
            {
                player.TakeDamage(damage, transform.parent.gameObject, transform.position);
            }
            
        }
    }
}
