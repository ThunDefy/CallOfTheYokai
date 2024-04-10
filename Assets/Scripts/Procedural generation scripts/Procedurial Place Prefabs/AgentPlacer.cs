using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AgentPlacer : MonoBehaviour
{
    [Header("Enemys")]
    [SerializeField]
    private List<EnemyScriptableObject> normalEnemys;

    [SerializeField]
    private List<float> normalEnemySpawnChance;

    [SerializeField]
    private List<EnemyScriptableObject> specialEnemys;

    [SerializeField]
    private List<float> specialEnemySpawnChance;

    [SerializeField]
    private EnemyScriptableObject bossEnemy;

    [Header("Placer data")]
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private int playerRoomIndex = 0;

    [SerializeField]
    private CinemachineVirtualCamera vCamera;

    [SerializeField]
    private int minRoomEnemiesCount=1;

    [SerializeField]
    private int maxRoomEnemiesCount=10;

    [SerializeField]
    private List<int> roomsWithEnemies = new List<int>();

    MapData dungeonData;

    [SerializeField]
    private bool showGizmo = false;

    private void Awake()
    {
        dungeonData = FindObjectOfType<MapData>();

    }



    public void PlaceAgents()
    {
        if (dungeonData == null)
            return;

        for (int i = 0; i < dungeonData.Rooms.Count; i++)
        {
            if (i != playerRoomIndex)
            {
                roomsWithEnemies.Add(Random.Range(minRoomEnemiesCount, maxRoomEnemiesCount + 1));
            }
            else
            {
                roomsWithEnemies.Add(0);
            }
        }
        //���� ��� ������ �������
        print(dungeonData.Rooms.Count);
        for (int i = 0; i < dungeonData.Rooms.Count; i++)
        {

            // ��� ���������� ������ ����� ���������������� ������ �������, ����� ����� ��, ������� �������� � ����
            Room room = dungeonData.Rooms[i];
            RoomGraph roomGraph = new RoomGraph(room.FloorTiles);

            //������� ���� � ���� ���������� �������
            HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>(room.FloorTiles);

            //������� ������, ����������� ��� � ����, ��� � � �������.
            roomFloor.IntersectWith(dungeonData.Path);

            //��������� BFS, ����� ����� ��� ������ � �������, ��������� � ��������
            Dictionary<Vector2Int, Vector2Int> roomMap = roomGraph.RunBFS(roomFloor.First(), room.PropPositions);

            //�������, �� ������� �� ����� ��������� + ���� = �������, ��� �� ����� ���������� ������
            room.PositionsAccessibleFromPath = roomMap.Keys.OrderBy(x => Guid.NewGuid()).ToList();

            //�������� �� �� ��� ������� � ������ RoomEnemiesCount?
            if (roomsWithEnemies.Count > i)
            {
                PlaceEnemies(room, roomsWithEnemies[i]);
            }

            //������� ������ 
            if (i == playerRoomIndex)
            { 
                //GameObject player = Instantiate(playerPrefab);
                player.transform.localPosition = dungeonData.Rooms[i].RoomCenterPos + Vector2.one * 0.5f;
                //���������� ������ ��������� �� �������
                vCamera.Follow = player.transform;
                vCamera.LookAt = player.transform;
                dungeonData.PlayerReference = player;
            }
        }
    }

    List<float> actualEnemySpawnChance = new List<float>();
    List<EnemyScriptableObject> actualEnemys = new List<EnemyScriptableObject>();
    // ����������� ������ �� �������, ��������� � ����
    private void PlaceEnemies(Room room, int enemysCount)
    {
        if(room.Type != RoomType.Boss)
        {
            if (room.Type == RoomType.Normal)
            {
                actualEnemySpawnChance = normalEnemySpawnChance;
                actualEnemys = normalEnemys;
            }
            else if (room.Type == RoomType.Treasure)
            {
                actualEnemySpawnChance = specialEnemySpawnChance;
                actualEnemys = specialEnemys;
            }

            float totalSpawnChance = 0f;
            foreach (float chance in actualEnemySpawnChance)
            {
                totalSpawnChance += chance;
            }

            for (int i = 0; i < enemysCount; i++)
            {
                if (room.PositionsAccessibleFromPath.Count <= i)
                {
                    return;
                }

                float randomValue = Random.Range(0f, totalSpawnChance);
                float cumulativeChance = 0f;

                for (int j = 0; j < actualEnemySpawnChance.Count; j++)
                {
                    cumulativeChance += actualEnemySpawnChance[j];
                    if (randomValue <= cumulativeChance)
                    {
                        GameObject enemy = Instantiate(actualEnemys[j].enemyPrefab);
                        enemy.transform.localPosition = (Vector2)room.PositionsAccessibleFromPath[i] + Vector2.one * 0.5f;
                        room.EnemiesInTheRoom.Add(enemy);
                        break;
                    }
                }
            }
        }
        else
        {
            GameObject enemy = Instantiate(bossEnemy.enemyPrefab);
            enemy.transform.localPosition = (Vector2)room.PositionsAccessibleFromPath[0] + Vector2.one * 0.5f;
            room.EnemiesInTheRoom.Add(enemy);
        }
        
        
        
    }

    // ���������� ����������� ��������� ��� ������ ������
    private void OnDrawGizmosSelected()
    {
        if (dungeonData == null || showGizmo == false)
            return;
        foreach (Room room in dungeonData.Rooms)
        {
            Color color = Color.green;
            color.a = 0.3f;
            Gizmos.color = color;

            foreach (Vector2Int pos in room.PositionsAccessibleFromPath)
            {
                Gizmos.DrawCube((Vector2)pos + Vector2.one * 0.5f, Vector2.one);
            }
        }
    }
}

public class RoomGraph
{
    public static List<Vector2Int> fourDirections = new()
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    Dictionary<Vector2Int, List<Vector2Int>> graph = new Dictionary<Vector2Int, List<Vector2Int>>();

    public RoomGraph(HashSet<Vector2Int> roomFloor)
    {
        foreach (Vector2Int pos in roomFloor)
        {
            List<Vector2Int> neighbours = new List<Vector2Int>();
            foreach (Vector2Int direction in fourDirections)
            {
                Vector2Int newPos = pos + direction;
                if (roomFloor.Contains(newPos))
                {
                    neighbours.Add(newPos);
                }
            }
            graph.Add(pos, neighbours);
        }
    }

    // ������� ����� ��������� ������
    public Dictionary<Vector2Int, Vector2Int> RunBFS(Vector2Int startPos, HashSet<Vector2Int> occupiedNodes)
    {
        //����������, ��������� � BFS
        Queue<Vector2Int> nodesToVisit = new Queue<Vector2Int>();
        nodesToVisit.Enqueue(startPos);

        HashSet<Vector2Int> visitedNodes = new HashSet<Vector2Int>();
        visitedNodes.Add(startPos);

        //�������, ������� �� ������ 
        Dictionary<Vector2Int, Vector2Int> map = new Dictionary<Vector2Int, Vector2Int>();
        map.Add(startPos, startPos);

        while (nodesToVisit.Count > 0)
        {
            //�������� ������ � ���������� �������
            Vector2Int node = nodesToVisit.Dequeue();
            List<Vector2Int> neighbours = graph[node];

            //���������� �� ������ �������� �������
            foreach (Vector2Int neighbourPosition in neighbours)
            {
                //�������� ������� ������ � ���� �����, ���� ��� �������������
                if (visitedNodes.Contains(neighbourPosition) == false &&
                    occupiedNodes.Contains(neighbourPosition) == false)
                {
                    nodesToVisit.Enqueue(neighbourPosition);
                    visitedNodes.Add(neighbourPosition);
                    map[neighbourPosition] = node;
                }
            }
        }

        return map;
    }

}
