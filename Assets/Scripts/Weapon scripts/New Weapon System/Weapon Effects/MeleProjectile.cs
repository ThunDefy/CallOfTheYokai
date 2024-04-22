using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleProjectile : Projectile
{
    private Transform parentTransform;
    public Vector3 scale;
    protected override void Start()
    {
        base.Start();

        parentTransform = transform.parent.parent.GetComponent<WeaponParent>().transform;
        if (parentTransform.localScale != new Vector3(1f, 1f, 0f))
        {
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x) * scale.x, Math.Abs(transform.localScale.x) * scale.y, scale.z);
        }
    }
    bool alreadyScaled = false;
    protected override void FixedUpdate()
    {

        base.FixedUpdate();

        // Проверяем, изменился ли масштаб родительского объекта по любой из осей
        if (parentTransform.localScale != new Vector3(1f, 1f, 0f) && alreadyScaled == false)
        {

            // Если да, компенсируем изменения масштаба, чтобы дочерний объект оставался в начальном масштабе
            transform.localScale = new Vector3(Math.Abs(transform.localScale.x) * scale.x, Math.Abs(transform.localScale.x) * scale.y, scale.z);
            alreadyScaled = true;
        }
        else if (parentTransform.localScale == new Vector3(1f, 1f, 0f)) alreadyScaled = false;



    }
}
