using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Prop", menuName = "ScriptableObjects/Prop")]
public class Prop : ScriptableObject
{
    [Header("Prop data:")]
    public GameObject[] PropPrefabs;
    public Sprite PropSprite;
    public Vector2Int PropSize = Vector2Int.one;
    public bool SpecialProp = false;
    public RoomType RoomType;


    [Space, Header("Placement type:")]
    public bool Corner = false;
    public bool NearWallUP = false;
    public bool NearWallDown = false;
    public bool NearWallRight = false;
    public bool NearWallLeft = false;
    public bool Inner = false;
    //public bool Center = false;
    [Min(0)]
    public int PlacementQuantityMin = 1;
    [Min(1)]
    public int PlacementQuantityMax = 1;

    [Space, Header("Group placement:")]
    public bool PlaceAsGroup = false;
    [Min(1)]
    public int GroupMinCount = 1;
    [Min(1)]
    public int GroupMaxCount = 1;

}
