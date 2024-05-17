using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KiseruRest : Pickup
{
    protected override void OnDestroy()
    {
        target.DoInstantRecharge();
    }
}
