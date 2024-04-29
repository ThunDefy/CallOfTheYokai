using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;
//using UnityEngine.WSA;
using Random = UnityEngine.Random;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap;
    //[SerializeField]
    //private List<TileBase> floorTiles;
    //[SerializeField]
    //private TileBase floorTile, wallTop, wallSideRight, wallSideLeft, wallBottom, wall, wallFloor,
    //    wallInnerCornerDownLeft, wallInnerCornerDownRight,
    //    wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft,
    //    wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft,
    //    wallTopRightEnd;

    private MapManager mapManager;
    BiomeData currentBiomeData;

    private void Start()
    {
        if (mapManager == null)
        {
            mapManager = FindObjectOfType<MapManager>(); 
        }
        UpdateLevelData();
    }

    public void UpdateLevelData()
    {
        LevelData currentLevelData = mapManager.GetCurrentLevelData();
        currentBiomeData = currentLevelData.biomeData;
    }

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, currentBiomeData.floorTiles);
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, List<TileBase> tiles)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tiles[Random.Range(0, tiles.Count)], position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    internal void PaintSingleBasicWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if (WallTypesHelper.wallTop.Contains(typeAsInt))
        {
            tile = currentBiomeData.wallTop;
        }
        else if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
        {
            tile = currentBiomeData.wallSideRight;
        }
        else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
        {
            tile = currentBiomeData.wallSideLeft;
        }
        else if (WallTypesHelper.wallBottm.Contains(typeAsInt))
        {
            tile = currentBiomeData.wallBottom;
        }
        else if (WallTypesHelper.wallFull.Contains(typeAsInt))
        {
            tile = currentBiomeData.wall;
        }

        if (tile != null)
        {
            PaintSingleTile(wallTilemap, tile, position);
            PaintSingleTile(floorTilemap, currentBiomeData.wallFloor, position);
        }
    }

    internal void PaintSingleCornerWall(Vector2Int position, string binaryType)
    {
        int typeASInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeASInt))
        {
            tile = currentBiomeData.wallInnerCornerDownLeft;
        }
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeASInt))
        {
            tile = currentBiomeData.wallInnerCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeASInt))
        {
            tile = currentBiomeData.wallDiagonalCornerDownLeft;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeASInt))
        {
            tile = currentBiomeData.wallDiagonalCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeASInt))
        {
            tile = currentBiomeData.wallDiagonalCornerUpRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeASInt))
        {
            tile = currentBiomeData.wallDiagonalCornerUpLeft;
        }
        else if (WallTypesHelper.wallFullEightDirections.Contains(typeASInt))
        {
            tile = currentBiomeData.wall;
        }
        else if (WallTypesHelper.wallBottmEightDirections.Contains(typeASInt))
        {
            tile = currentBiomeData.wallBottom;
        }
        else if (WallTypesHelper.wallTopRightEnd.Contains(typeASInt))
        {
            tile = currentBiomeData.wallTopRightEnd;
        }

        if (tile != null)
        {
            PaintSingleTile(wallTilemap, tile, position);
            PaintSingleTile(floorTilemap, currentBiomeData.wallFloor, position);
        }
    }
}

