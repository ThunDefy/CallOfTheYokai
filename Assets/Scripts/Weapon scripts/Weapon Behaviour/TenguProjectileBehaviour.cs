using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenguProjectileBehaviour : ProjectileWeaponBehaviour
{
    TenguController tc;

    protected override void Start()
    {
        base.Start();
        tc = FindObjectOfType<TenguController>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * tc.speed * Time.deltaTime;
        DetectColliders();
    }

}
