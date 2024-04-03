using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveEffectScriptableObject", menuName = "ScriptableObjects/Passive Effect")]
public class PassiveEffectScriptableObject : ScriptableObject
{
    [SerializeField]
    float multipler;
    int level;
    public string passiveEffectName;
    public string passiveEffectDescription;
    public float Multipler { get => multipler; private set => multipler = value; }
}
