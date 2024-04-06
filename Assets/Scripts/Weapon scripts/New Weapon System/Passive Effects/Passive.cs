using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passive : Yokai
{
    public PassiveData data; 
    [SerializeField] PlayerData.Stats currentBoosts;

    [System.Serializable]

    public struct Modifier
    {
        public string name, description;
        public PlayerData.Stats boosts;
    }

    public virtual void Initialise(PassiveData data)
    {
        base.Initialise(data);
        this.data = data;
        currentBoosts = data.baseStats.boosts;
    }

    public virtual PlayerData.Stats GetBoosts()
    {
        return currentBoosts;
    }

    public override bool DoLevelUp(int upgradeIndx)
    {
        base.DoLevelUp(upgradeIndx);
        if (!CanLevelUp())
        {
            return false;
        }
        currentBoosts += data.GetLevelData(upgradeIndx).boosts;
        return true;
    }
}
