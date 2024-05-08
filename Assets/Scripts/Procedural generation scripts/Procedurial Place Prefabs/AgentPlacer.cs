using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AgentPlacer : MonoBehaviour
{
    //[Header("Enemys")]
    //[SerializeField]
    //private List<EnemyScriptableObject> normalEnemys;

    //[SerializeField]
    //private List<float> normalEnemySpawnChance;

    //[SerializeField]
    //private List<EnemyScriptableObject> specialEnemys;

    //[SerializeField]
    //private List<float> specialEnemySpawnChance;

    //[SerializeField]
    //private EnemyScriptableObject bossEnemy;

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
    private int guardiansCount = 2;

    [SerializeField]
    private List<int> roomsWithEnemies = new List<int>();

    MapData dungeonData;
    MapManager mapManager;
    LevelData currentLevelData;

    [SerializeField]
    private bool showGizmo = false;


    private void Awake()
    {
        dungeonData = FindObjectOfType<MapData>();
    }

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
        currentLevelData = mapManager.GetCurrentLevelData();
        minRoomEnemiesCount = currentLevelData.minRoomEnemiesCount;
        maxRoomEnemiesCount = currentLevelData.maxRoomEnemiesCount;
        guardiansCount = currentLevelData.guardiansCount;
    }

    public void PlaceAgents()
    {
        if (dungeonData == null)
            return;

        for (int i = 0; i < dungeonData.Rooms.Count; i++)
        {
            if (i != playerRoomIndex)
            {
                if (dungeonData.Rooms[i].Type == RoomType.Treasure)
                {
                    roomsWithEnemies.Add(guardiansCount);
                }else
                    roomsWithEnemies.Add(Random.Range(minRoomEnemiesCount, maxRoomEnemiesCount + 1));
            }
            else
            {
                roomsWithEnemies.Add(0);
            }
        }
        //÷икл дл€ каждой комнаты
        print(dungeonData.Rooms.Count);
        for (int i = 0; i < dungeonData.Rooms.Count; i++)
        {

            // ƒл€ размещени€ врагов нужно проанализировать плитки комнаты, чтобы найти те, которые доступны с пути
            Room room = dungeonData.Rooms[i];
            RoomGraph roomGraph = new RoomGraph(room.FloorTiles);

            //Ќайдите ѕуть в этой конкретной комнате
            HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>(room.FloorTiles);

            //Ќайдите плитки, относ€щиес€ как к пути, так и к комнате.
            roomFloor.IntersectWith(dungeonData.Path);

            //«апустить BFS, чтобы найти все плитки в комнате, доступной с тропинки
            Dictionary<Vector2Int, Vector2Int> roomMap = roomGraph.RunBFS(roomFloor.First(), room.PropPositions);

            //ѕозиции, до которых мы можем добратьс€ + путь = позиции, где мы можем разместить врагов
            room.PositionsAccessibleFromPath = roomMap.Keys.OrderBy(x => Guid.NewGuid()).ToList();

            //добавили ли мы эту комнату в список RoomEnemiesCount?
            if (roomsWithEnemies.Count > i)
            {
                if (room.Type == RoomType.Treasure)
                {
                    if (i != playerRoomIndex)
                        PlaceEnemies(room, guardiansCount);
                }
                else
                    PlaceEnemies(room, roomsWithEnemies[i]);
            }

            //—павним игрока 
            if (i == playerRoomIndex)
            { 
                //GameObject player = Instantiate(playerPrefab);
                player.transform.localPosition = dungeonData.Rooms[i].RoomCenterPos + Vector2.one * 0.5f;
                //«аставл€ем камеру следовать за игроком
                vCamera.Follow = player.transform;
                vCamera.LookAt = player.transform;
                dungeonData.PlayerReference = player;
            }
        }
    }

    List<float> actualEnemySpawnChance = new List<float>();
    List<EnemyScriptableObject> actualEnemys = new List<EnemyScriptableObject>();
    // –асставл€ет врагов на позиции, доступные с пути
    private void PlaceEnemies(Room room, int enemysCount)
    {
        if(room.Type != RoomType.Boss)
        {
            if (room.Type == RoomType.Normal)
            {
                actualEnemySpawnChance = currentLevelData.normalEnemySpawnChance;
                actualEnemys = currentLevelData.normalEnemys;
            }
            else if (room.Type == RoomType.Treasure)
            {
                actualEnemySpawnChance = currentLevelData.specialEnemySpawnChance;
                actualEnemys = currentLevelData.specialEnemys;
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
                        enemy.GetComponentInChildren<WeaponParent>().mapLevel = mapManager.CurrentLevelIndex; ///////////////////////////////
                        enemy.GetComponent<Health>().SetMaxHealth(actualEnemys[j].healthStats[mapManager.CurrentLevelIndex]);
                        enemy.transform.localPosition = (Vector2)room.PositionsAccessibleFromPath[i] + Vector2.one * 0.5f;
                        room.EnemiesInTheRoom.Add(enemy);

                        break;
                    }
                }
            }
        }
        else
        {
            GameObject enemy = Instantiate(currentLevelData.bossEnemy.enemyPrefab);
            enemy.GetComponentInChildren<WeaponParent>().mapLevel = mapManager.CurrentLevelIndex;
            enemy.GetComponent<Health>().SetMaxHealth(currentLevelData.bossEnemy.healthStats[mapManager.CurrentLevelIndex]);
            enemy.transform.localPosition = (Vector2)room.PositionsAccessibleFromPath[0] + Vector2.one * 0.5f;
            room.EnemiesInTheRoom.Add(enemy);
        }
        
        
        
    }

    // ¬изуальное отображение доступных дл€ спавна плиток
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

    // —оздает карту доступных тайлов
    public Dictionary<Vector2Int, Vector2Int> RunBFS(Vector2Int startPos, HashSet<Vector2Int> occupiedNodes)
    {
        //ѕеременные, св€занные с BFS
        Queue<Vector2Int> nodesToVisit = new Queue<Vector2Int>();
        nodesToVisit.Enqueue(startPos);

        HashSet<Vector2Int> visitedNodes = new HashSet<Vector2Int>();
        visitedNodes.Add(startPos);

        //—ловарь, который мы вернем 
        Dictionary<Vector2Int, Vector2Int> map = new Dictionary<Vector2Int, Vector2Int>();
        map.Add(startPos, startPos);

        while (nodesToVisit.Count > 0)
        {
            //получить данные о конкретной позиции
            Vector2Int node = nodesToVisit.Dequeue();
            List<Vector2Int> neighbours = graph[node];

            //пройдитесь по каждой соседней позиции
            foreach (Vector2Int neighbourPosition in neighbours)
            {
                //добавить позицию соседа в нашу карту, если она действительна
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
