using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerCollector : MonoBehaviour
{
    PlayerStats player;
    CircleCollider2D magnetCollider;
    public float pullSpeed;

    private void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        //magnetCollider = GetComponent<CircleCollider2D>();
    }

    //private void Update()
    //{
    //    magnetCollider.radius = player.currentMagnet;
    //}

    public void SetRadius(float r)
    {
        if(!magnetCollider) magnetCollider = GetComponent<CircleCollider2D>();
        magnetCollider.radius = r;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Pickup p))
        {
            //Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            //Vector2 forceDirection = (transform.position - collision.transform.position).normalized;
            //float angle = Mathf.Atan2(forceDirection.y, forceDirection.x) * Mathf.Rad2Deg;
            //collision.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            //rb.AddForce(forceDirection * pullSpeed);

            p.Collect(player,pullSpeed);
        }
    }


    //void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.gameObject.TryGetComponent(out ICollectible collectible))
    //    {
    //        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
    //        Vector2 forceDirection = (transform.position - collision.transform.position).normalized;
    //        float angle = Mathf.Atan2(forceDirection.y, forceDirection.x) * Mathf.Rad2Deg;
    //        collision.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    //        rb.AddForce(-forceDirection * pullSpeed);
    //        collectible.Collect();
    //    }
    //}

}
