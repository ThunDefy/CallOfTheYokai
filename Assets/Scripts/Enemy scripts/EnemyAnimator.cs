using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    Animator am;
    Agent pc;
    SpriteRenderer sr;



    void Start()
    {
        am = GetComponent<Animator>();
        pc = GetComponent<Agent>();
        sr = GetComponent<SpriteRenderer>();
    }


    void Update()
    {

        if (pc.moveDir.x != 0 || pc.moveDir.y != 0)
        {
            am.SetFloat("xDir", 0);
            am.SetFloat("yDir", 0);
            am.SetLayerWeight(1, 1);
            if(pc.moveDir.x > 0 || pc.moveDir.x < 0) am.SetFloat("xDir", pc.moveDir.x);
            else if (pc.moveDir.y > 0 || pc.moveDir.y < 0) am.SetFloat("yDir", pc.moveDir.y);

            //print("X = "+am.GetFloat("xDir")+" Y = "+ am.GetFloat("yDir"));

        }
        else
        {
            am.SetLayerWeight(1, 0);
        }

    }

}
