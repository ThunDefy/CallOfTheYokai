using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenguPassiveEffect : PassiveEffect
{
    public override void ApplyModifier()
    {
        player.CurrentMoveSpeed = player.player.moveSpeed *( 1 + multipler / 100f);
    }
}
