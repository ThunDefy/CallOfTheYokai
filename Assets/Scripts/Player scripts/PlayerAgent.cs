using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAgent : Agent
{
    public PlayerData player;
    public PlayerStats playerData;
    private Health hpData;
    //protected override void Move()
    //{
    //    rb.velocity = new Vector2(moveDir.x * playerData.currentMoveSpeed, moveDir.y * moveSpeed);
    //}
    protected override void Awake()
    {
        base.Awake();
        hpData = GetComponent<Health>();
        hpData.maxHealth = player.stats.maxHealth;
        hpData.currentHealth = hpData.maxHealth;
    }
}
