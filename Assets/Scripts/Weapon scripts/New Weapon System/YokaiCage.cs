using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YokaiCage : MonoBehaviour
{
    private SpriteAnimation anim;
    public List<PlayerWeaponData> posibleYokais;
    PlayerInventory playerInventory;

    private void Awake()
    {
        anim = GetComponent<SpriteAnimation>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && (playerInventory = collision.GetComponent<PlayerInventory>()))
        {
            if (playerInventory!=null) anim.EndingAnimation();
            else Debug.LogError("Player don't have inventory");
        }
    }

    public void GiveYokaiToPlayer()
    {
        int rndmIndexOfYokai = ChoseRandomYokai();
        int result = playerInventory.AddYokai(posibleYokais[rndmIndexOfYokai]);

        if(result == -2)
        {
            //Debug.LogWarning("У тебя уже есть этот Ёкай");
        }else if (result != -1)
        {
            Destroy(gameObject);
        }
        else
        {
        }
        Destroy(gameObject);

    }
    private int ChoseRandomYokai()
    {
        return Random.Range(0, posibleYokais.Count);
    }

}
