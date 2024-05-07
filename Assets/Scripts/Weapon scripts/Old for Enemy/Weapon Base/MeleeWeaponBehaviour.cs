using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponBehaviour : MonoBehaviour
{
    //public Transform circleOrigin;
    public WeaponScriptableObject weaponData;
    public float radius;
    public float xOffset = 0f;
    public float yOffset = 0f;
    public bool drawGizmo;
    public bool agentIsCenter = true;

    GameObject childCollider;
    Vector3 center;

    private void OnDrawGizmosSelected()
    {
        if (drawGizmo) { 
            Gizmos.color = Color.blue;
            //Vector3 position = circleOrigin == null ? Vector3.zero : circleOrigin.position;
            Vector3 position;
            if (!agentIsCenter)
                position = new Vector3(childCollider.transform.localPosition.x, childCollider.transform.localPosition.y , childCollider.transform.localPosition.z);
            else position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            
            Gizmos.DrawWireSphere(position, radius);
        }
    }
    
    
    private void Start()
    {
        if (!agentIsCenter)
        {
            childCollider = new GameObject("DetectCollider");
            childCollider.transform.parent = transform; // установить родителя для нового GameObject
            CircleCollider2D colliderComponent = childCollider.AddComponent<CircleCollider2D>();
            colliderComponent.radius = radius;
            colliderComponent.isTrigger = true;
            childCollider.transform.localPosition = new Vector3(transform.localPosition.x + xOffset, transform.localPosition.y + yOffset, transform.localPosition.z);
        }
    }

    public void DetectColliders(float damage,string target)
    {
        if (!agentIsCenter)
        {
            center = childCollider.transform.position;
        }
        else center = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        foreach (Collider2D collider in Physics2D.OverlapCircleAll(center, radius))
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
                    Health targetHealth = collider.GetComponent<Health>();
                    if (targetHealth)
                    {
                        //print("Ebat chertey");
                        //targetHealth.GetHit(damage, transform.parent.gameObject, transform.position, weaponData.knockback);
                        targetHealth.GetHit(damage, transform.parent.gameObject, transform.position);
                    }
                } 
                
            }
            
        }
    }
}
