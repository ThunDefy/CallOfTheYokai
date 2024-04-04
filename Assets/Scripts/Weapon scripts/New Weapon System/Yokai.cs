using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Yokai : MonoBehaviour
{
    public int currentLevel = 1, maxLevel = 1;

    protected PlayerStats owner;

    public virtual void Initialise(YokaiData data)
    {
        maxLevel = data.maxLevel;
        owner = FindObjectOfType<PlayerStats>();
    }

    public virtual bool CanLevelUp()
    {
        return currentLevel <= maxLevel;
    }

    public virtual bool DoLevelUp()
    {
        return true;
    }

    public virtual void OnEquip()
    {

    }

    public virtual void OnUnEquip() { }
}
