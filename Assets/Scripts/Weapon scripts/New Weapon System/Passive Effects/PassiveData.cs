using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveData", menuName = "ScriptableObjects/YokaiPassiveEffect")]
public class PassiveData : YokaiData
{

    public Passive.Modifier baseStats;
    public Passive.Modifier[] growth;

    public Passive.Modifier GetLevelData(int indx)
    {
        //if (level - 2 < growth.Length) return growth[level - 2];
        //return new Passive.Modifier();
        return growth[indx];
    }

    public int GetRandomLevelData()
    {
        return Random.Range(0, growth.Length);
    }
}
