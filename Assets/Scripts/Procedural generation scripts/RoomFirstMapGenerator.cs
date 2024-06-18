using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class RoomFirstMapGenerator : SimpleRandomWalkMapGenerator
{
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField]
    [Range(0, 10)]
    private int offset = 1;
    [SerializeField]
    private bool randomWalkRooms = false;

    public UnityEvent OnFinishedRoomGeneration;

    private MapData mapData;
    MapManager mapManager;
    LevelData currentLevelData;

    int currentTresureRoomCount;
    List<int> treasureRoomIndexes = new List<int>();

    private void Start()
    {
        GameManager.instance.StartLoading();
        mapManager = FindObjectOfType<MapManager>();
        UpdateLevelData();
        GenerateMap();
        
    }
    public void UpdateLevelData()
    {
        currentLevelData = mapManager.GetCurrentLevelData();
        currentTresureRoomCount = currentLevelData.tresureRoomCount;
        if (mapManager.CurrentLevelIndex == 0) currentTresureRoomCount -= 1;

        randomWalkParameters = currentLevelData.randomWalkParameters;
        minRoomWidth = currentLevelData.minRoomWidth;
        minRoomHeight = currentLevelData.minRoomHeight;
        dungeonWidth = currentLevelData.dungeonWidth;
        dungeonHeight = currentLevelData.dungeonHeight;
        offset = currentLevelData.offset;
    }
    protected override void RunProceduralGeneration()
    {
        
        mapData = FindObjectOfType<MapData>();
        if (mapData == null)
            mapData = gameObject.AddComponent<MapData>();

        CreateRooms();
        
    }

    private void CreateRooms()
    {
        mapData.Reset();

        var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        RandomlyPlaceTresures(roomsList.Count);

        if (randomWalkRooms)
        {
            floor = CreateRoomsRandomly(roomsList, mapData);
            
        }
        else
        {
            floor = CreateSimpleRooms(roomsList);
        }


        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters, mapData);

        HashSet<Vector2Int> newCorridor = new HashSet<Vector2Int>(IncreaseCorridorSizeBy3(corridors));

        floor.UnionWith(newCorridor);


        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);

        OnFinishedRoomGeneration?.Invoke();

    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }


    private void RandomlyPlaceTresures(int roomCount)
    {
        System.Random random = new System.Random(); 

        while (treasureRoomIndexes.Count < currentTresureRoomCount) 
        {
            int index = random.Next(2, roomCount-1); 
            if (!treasureRoomIndexes.Contains(index)) 
            {
                treasureRoomIndexes.Add(index); 
            }
        }
    }
    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList, MapData mapData)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            HashSet<Vector2Int> roomTiles = new();

            foreach (var position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) && 
                    position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset) )
                {
                    floor.Add(position);
                    roomTiles.Add((Vector2Int)position);
                }    
            }
            if (i == 0 && mapManager.CurrentLevelIndex == 0) // Спавн первого оружия
            {
                mapData.Rooms.Add(new Room(roomCenter, roomTiles, RoomType.Treasure));
            }
            else if (treasureRoomIndexes.Contains(i)) // Проверьте, содержит ли текущий индекс комнаты сокровища
            {
                mapData.Rooms.Add(new Room(roomCenter, roomTiles, RoomType.Treasure)); // Добавьте комнату с сокровищами
            }
            else if (i == roomsList.Count - 1)
            {
                if (currentLevelData.haveBoss)
                    mapData.Rooms.Add(new Room(roomCenter, roomTiles, RoomType.Boss));
                else
                {
                    mapData.Rooms.Add(new Room(roomCenter, roomTiles, RoomType.Portal));
                }
            }
               
            else mapData.Rooms.Add(new Room(roomCenter, roomTiles, RoomType.Normal));
        }

        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters, MapData mapData)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
            mapData.Path.UnionWith(newCorridor);
        }
        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
        }
        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> IncreaseCorridorSizeBy3(HashSet<Vector2Int> corridors)
    {
        HashSet<Vector2Int> newCorridor = new HashSet<Vector2Int>();
        foreach (Vector2Int point in corridors)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    newCorridor.Add(point + new Vector2Int(x, y));
                }
            }
        }
        return newCorridor;
    }
}
