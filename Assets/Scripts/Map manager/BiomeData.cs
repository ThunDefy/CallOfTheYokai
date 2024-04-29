using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "BiomeData", menuName = "ScriptableObjects/BiomeData")]
public class BiomeData : ScriptableObject
{
    public List<TileBase> floorTiles;
    public TileBase floorTile, wallTop, wallSideRight, wallSideLeft, wallBottom, wall, wallFloor,
        wallInnerCornerDownLeft, wallInnerCornerDownRight,
        wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft,
        wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft,
        wallTopRightEnd;

}
