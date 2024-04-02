using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenguProjectileBehaviour : ProjectileWeaponBehaviour
{

    protected override void Start()
    {
        base.Start();

    }

    void Update()
    {
        transform.position += direction * weaponData.currentSpeed * Time.deltaTime;
        DetectColliders();
    }

}
