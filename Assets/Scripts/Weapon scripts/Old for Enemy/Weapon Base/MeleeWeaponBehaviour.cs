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
            print(collider.tag + " ? "+ target);
            if (collider.tag == target)
            {

                PlayerStats player = collider.GetComponent<PlayerStats>();
                if (player != null)
                {
                    
                    player.TakeDamage(damage, transform.parent.gameObject, transform.position);
                }
                else
                {
                    collider.GetComponent<Health>().GetHit(damage, transform.parent.gameObject, transform.position);
                }
            }
            
        }
    }
}
