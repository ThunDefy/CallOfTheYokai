using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Direction2D;

public static class ProceduralGenerationAlgorithms 
{
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();
        path.Add(startPosition);
        var previousposition = startPosition;
        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousposition + Direction2D.GetRandomCardinalDirection();
            path.Add(newPosition);
            previousposition = newPosition;
        }
        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        var direction = Direction2D.GetRandomCardinalDirection();
        var currentPosition = startPosition;
        corridor.Add(currentPosition);

        for (int i = 0; i < corridorLength; i++)
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }
        return corridor;
    }

    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        BoundsInt initialPoint;
        initialPoint = new BoundsInt(0, 0,0,0,0,0);
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();
        roomsQueue.Enqueue(spaceToSplit);

        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            if (room.size.y >= minHeight && room.size.x >= minWidth)
            {
                if (Random.value < 0.5f)
                {
                    if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    else if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    else 
                    {
                        roomsList.Add(room);
                    }
                }
                else
                {
                    if (room.size.x >= minWidth * 2)
                    {
                        SplitVertically(minWidth, roomsQueue, room);
                    }
                    else if (room.size.y >= minHeight * 2)
                    {
                        SplitHorizontally(minHeight, roomsQueue, room);
                    }
                    else 
                    {
                        roomsList.Add(room);
                    }
                }
            }
        }
        RoomDistanceComparer comparer = new RoomDistanceComparer(roomsList[0]);
        roomsList.Sort(comparer);
        return roomsList;
    }

    private static void SplitVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
            new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(1, room.size.y);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
            new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }


}

public static class Direction2D
{
    public static List<Vector2Int> cardinalDirectionList = new List<Vector2Int>
    {
        new Vector2Int(0,1), // Вверх
        new Vector2Int(1,0), // Вправо
        new Vector2Int(0,-1), // Вниз
        new Vector2Int(-1,0) // Влево
    };

    public static List<Vector2Int> diagonalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(1,1), //Вверх-вправо
        new Vector2Int(1,-1), //Вправо-вниз
        new Vector2Int(-1, -1), // Вниз-влево
        new Vector2Int(-1, 1) //Влево-вверх
    };

    public static List<Vector2Int> eightDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0,1), // Вверх
        new Vector2Int(1,1), //Вверх-вправо
        new Vector2Int(1,0), // Вправо
        new Vector2Int(1,-1), //Вправо-вниз
        new Vector2Int(0,-1), // Вниз
        new Vector2Int(-1, -1), // Вниз-влево
        new Vector2Int(-1,0), // Влево
        new Vector2Int(-1, 1) //Влево-вверх

    };

    public static Vector2Int GetRandomCardinalDirection()
    {
        return cardinalDirectionList[UnityEngine.Random.Range(0, cardinalDirectionList.Count)];
    }


    // Код для сортировки комнат 
    public static class RoomDistanceComparator
    {
        public static float CalculateDistance(Vector2Int point1, Vector2Int point2)
        {
            return Vector2Int.Distance(point1, point2);
        }
    }

    public class RoomDistanceComparer : IComparer<BoundsInt>
    {
        private BoundsInt previousRoom;

        public RoomDistanceComparer(BoundsInt prevRoom)
        {
            previousRoom = prevRoom;
        }

        public int Compare(BoundsInt room1, BoundsInt room2)
        {
            Vector2Int initialPoint = new Vector2Int(Mathf.RoundToInt(previousRoom.center.x), Mathf.RoundToInt(previousRoom.center.y));

            float distance1 = RoomDistanceComparator.CalculateDistance(initialPoint, new Vector2Int(Mathf.RoundToInt(room1.center.x), Mathf.RoundToInt(room1.center.y)));
            float distance2 = RoomDistanceComparator.CalculateDistance(initialPoint, new Vector2Int(Mathf.RoundToInt(room2.center.x), Mathf.RoundToInt(room2.center.y)));

            return distance1.CompareTo(distance2);
        }
    }

}
