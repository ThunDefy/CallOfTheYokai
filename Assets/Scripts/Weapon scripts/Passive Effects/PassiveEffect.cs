using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PassiveEffect : MonoBehaviour
{
    protected PlayerStats player;
    public PassiveEffectScriptableObject passiveEffectData;
    public float multipler;

    void Start()
    {
        multipler = passiveEffectData.Multipler;
        player = FindAnyObjectByType<PlayerStats>();
        ApplyModifier();
    }

    public virtual void ApplyModifier()
    {

    }
    
}
