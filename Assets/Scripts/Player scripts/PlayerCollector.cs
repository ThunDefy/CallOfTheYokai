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
    }


    public void SetRadius(float r)
    {
        if(!magnetCollider) magnetCollider = GetComponent<CircleCollider2D>();
        magnetCollider.radius = r;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Pickup p))
        {
            p.Collect(player,pullSpeed);
        }
    }

}
