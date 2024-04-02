using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PassiveEffect : MonoBehaviour
{
    protected PlayerStats player;
    public PassiveEffectScriptableObject passiveEffectData;
    public float multipler;

    public virtual void ApplyModifier()
    {

    }
    void Start()
    {
        multipler = passiveEffectData.Multipler;
        player = FindAnyObjectByType<PlayerStats>();
        ApplyModifier();
    }

}
