using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenguPassiveEffect : PassiveEffect
{
    public override void ApplyModifier()
    {
        //player.playerData.moveSpeed *= 1 + passiveEffectData.Multipler / 100f; // Добавляется проценты к скорости передвижения
        //player.CurrentMoveSpeed = player.playerData.moveSpeed;

        player.CurrentMoveSpeed *= 1 + passiveEffectData.Multipler / 100f;
    }
}
