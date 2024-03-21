using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIData : MonoBehaviour
{
    public List<Transform> targets = null;
    public Collider2D[] obstacles = null; // ссылки на препятсвтия 

    public Transform currentTarget; // текущая цель преследования

    public int GetTargetsCount() => targets == null ? 0 : targets.Count;
}
