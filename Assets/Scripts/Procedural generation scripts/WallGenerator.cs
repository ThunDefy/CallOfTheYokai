using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator 
{
    public static void CreateWalls(HashSet<Vector2Int> floorPosition, TilemapVisualizer tilemapVisualizer)
    {
        var basicWallPositions = FindWallsInDirections(floorPosition, Direction2D.cardinalDirectionList);
        var cornerWallPositions = FindWallsInDirections(floorPosition, Direction2D.cardinalDirectionList);
        CreateBasicWall(tilemapVisualizer, basicWallPositions, floorPosition);
        CreateCornerWall(tilemapVisualizer, cornerWallPositions, floorPosition);
    }

    private static void CreateCornerWall(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in cornerWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in Direction2D.eightDirectionsList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                {
                    neighboursBinaryType += "1";
                }
                else
                {
                    neighboursBinaryType += "0";
                }
            }
            tilemapVisualizer.PaintSingleCornerWall(position, neighboursBinaryType);
        }
    }

    private static void CreateBasicWall(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> floorPosition)
    {
        foreach (var position in basicWallPositions)
        {
            string neighborsBinType = "";
            foreach (var direction in Direction2D.cardinalDirectionList)
            {
                var neighborPosition = position + direction;
                if (floorPosition.Contains(neighborPosition))
                {
                    neighborsBinType += "1";
                }
                else
                {
                    neighborsBinType += "0";
                }
            }
            tilemapVisualizer.PaintSingleBasicWall(position, neighborsBinType);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPosition, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var position in floorPosition)
        {
            foreach (var direction in directionList)
            {
                var neighbourPosition = position + direction;
                if(floorPosition.Contains(neighbourPosition) == false) // для углов
                    wallPositions.Add(neighbourPosition);
            }
        }
        return wallPositions;
    }
}
