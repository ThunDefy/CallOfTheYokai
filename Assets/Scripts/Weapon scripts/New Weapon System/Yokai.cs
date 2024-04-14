using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Yokai : MonoBehaviour
{
    public int currentLevel = 1;
    public int currentRisingLevel = 0, maxRisingLevel = 5;

    protected PlayerStats owner;

    public PlayerStats Owner { get { return owner; } }

    public virtual void Initialise(YokaiData data)
    {
        maxRisingLevel = data.maxRisingLevel;
        owner = FindObjectOfType<PlayerStats>();
    }

    public virtual bool CanLevelUp()
    {
        if (currentRisingLevel <= maxRisingLevel) return true;
        else
        {
            currentRisingLevel = maxRisingLevel;
            return false;
        }

    }

    public virtual bool DoLevelUp(int upgradeIndx)
    {
        return true;
    }
    public virtual bool RisingUpYokai()
    {
        return true;
    }
    public virtual void OnEquip(){ }

    public virtual void OnUnEquip() { }
}
