using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PropPlacementManager : MonoBehaviour
{
    MapData dungeonData;
    MapManager mapManager;
    LevelData currentLevelData;

    //[SerializeField]
    //private List<Prop> propsToPlace;

    [SerializeField, Range(0, 1)]
    private float cornerPropPlacementChance = 0.7f; // ���� ���������� ������� � ����

    [SerializeField]
    private GameObject propPrefab;

    public UnityEvent OnFinished;

    PlayerStats ps;

    private void Awake()
    {
        dungeonData = FindObjectOfType<MapData>();
        ps = FindObjectOfType <PlayerStats>();
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
    }

    public void ProcessRooms()
    {
        if (dungeonData == null)
            return;

        foreach (Room room in dungeonData.Rooms)
        {
            //������ �������
            List<Prop> innerProps = currentLevelData.propsToPlace.Where(x => x.Inner).OrderByDescending(x => x.PropSize.x * x.PropSize.y).ToList();
            if (innerProps.Count != 0) PlaceProps(room, innerProps, room.InnerTiles, PlacementOriginCorner.BottomLeft);

            //���������� ������� �� �����
            List<Prop> cornerProps = currentLevelData.propsToPlace.Where(x => x.Corner).ToList();
            if(cornerProps.Count!=0) PlaceCornerProps(room, cornerProps);

            //����� ����� �����
            List<Prop> leftWallProps = currentLevelData.propsToPlace.Where(x => x.NearWallLeft).OrderByDescending(x => x.PropSize.x * x.PropSize.y).ToList();
            if (leftWallProps.Count != 0) PlaceProps(room, leftWallProps, room.NearWallTilesLeft, PlacementOriginCorner.BottomLeft);

            //����� ������ �����
            List<Prop> rightWallProps = currentLevelData.propsToPlace.Where(x => x.NearWallRight).OrderByDescending(x => x.PropSize.x * x.PropSize.y).ToList();
            if (rightWallProps.Count != 0) PlaceProps(room, rightWallProps, room.NearWallTilesRight, PlacementOriginCorner.TopRight);

            //����� ������� �����
            List<Prop> topWallProps = currentLevelData.propsToPlace.Where(x => x.NearWallUP).OrderByDescending(x => x.PropSize.x * x.PropSize.y).ToList();
            if (topWallProps.Count != 0) PlaceProps(room, topWallProps, room.NearWallTilesUp, PlacementOriginCorner.TopLeft);
            print("topWallProps.Count = " + topWallProps.Count);

            //����� ������ �����
            List<Prop> downWallProps = currentLevelData.propsToPlace.Where(x => x.NearWallDown).OrderByDescending(x => x.PropSize.x * x.PropSize.y).ToList();
            if (downWallProps.Count != 0) PlaceProps(room, downWallProps, room.NearWallTilesDown, PlacementOriginCorner.BottomLeft);
        }

        Invoke("RunEvent", 1);

    }

    public void RunEvent()
    {
        OnFinished?.Invoke();
    }

    
    // ��������� ������� ����� ����. ��� ����� ������� �������� � ����� ������ ����������
    private void PlaceProps(Room room, List<Prop> wallProps, HashSet<Vector2Int> availableTiles, PlacementOriginCorner placement)
    {
        //������� ������� ���� �� ��������� ������ nearWallTiles, ����� ���������� ��������� ���� ��� ����������� ����������
        HashSet<Vector2Int> tempPositons = new HashSet<Vector2Int>(availableTiles);
        tempPositons.ExceptWith(dungeonData.Path);

        //����������� ���������� ��� �������
        foreach (Prop propToPlace in wallProps)
        {
            //����� ���������� ������ ������������ ���������� ������� �������
            int quantity = UnityEngine.Random.Range(propToPlace.PlacementQuantityMin, propToPlace.PlacementQuantityMax + 1);


            // ����� ����� ������ � � ����� ��������� ��������� 1 ��� ����� �������
            if (propToPlace.isInfluenceOfLuck) quantity += UnityEngine.Random.value < ps.actualStats.luck ? 1 : 0 ;

            if (propToPlace.RoomType == room.Type || propToPlace.RoomType == RoomType.All)
            {
                for (int i = 0; i < quantity; i++)
                {
                    tempPositons.ExceptWith(room.PropPositions);
                    List<Vector2Int> availablePositions = tempPositons.OrderBy(x => Guid.NewGuid()).ToList();
                    if (TryPlacingPropBruteForce(room, propToPlace, availablePositions, placement) == false)break;
                }
            }
            
            //else
            //{
            //    for (int i = 0; i < quantity; i++)
            //    {
            //        //������� ������� �������
            //        tempPositons.ExceptWith(room.PropPositions);
            //        //������������ �������
            //        List<Vector2Int> availablePositions = tempPositons.OrderBy(x => Guid.NewGuid()).ToList();
            //        //���� ���������� �� �������, ��� ������ �������� ���������� ��� �� ����� �������� �����
            //        if (TryPlacingPropBruteForce(room, propToPlace, availablePositions, placement) == false)break;
            //    }
            //}
            

        }
    }


    // �������� ���������� ������, ��������� ������ ���� (������ ������ ��������� ������� ������).
    private bool TryPlacingPropBruteForce(Room room, Prop propToPlace, List<Vector2Int> availablePositions, PlacementOriginCorner placement)
    {
        //��������� ���������� �������, ������� � ����, ���������� ���������� placement
        for (int i = 0; i < availablePositions.Count; i++)
        {
            //�������� ��������� ������� (�� ��� ����� ���� ��� ������ ����� ���������� ������� ���������� � ���� ������)
            Vector2Int position = availablePositions[i];
            if (room.PropPositions.Contains(position))
                continue;

            //���������, ���������� �� ����� ������ ��� ��������� �������
            List<Vector2Int> freePositionsAround
                = TryToFitProp(propToPlace, availablePositions, position, placement);

            //���� � ��� ����� ���������� �����, ���������� ������
            if (freePositionsAround.Count == propToPlace.PropSize.x * propToPlace.PropSize.y)
            {
                //��������� ������� ������
                PlacePropGameObjectAt(room, position, propToPlace);
                //�������������� ��� �������, ��������� �������� (� ����������� �� ��� �������).
                foreach (Vector2Int pos in freePositionsAround)
                {
                    //Hashest ����� ������������ ����������� �������
                    room.PropPositions.Add(pos);
                }

                //���������� � �������� ��������
                if (propToPlace.PlaceAsGroup)
                {
                    PlaceGroupObject(room, position, propToPlace, 1);
                }
                return true;
            }
        }

        return false;
    }

    // ��������, �������� �� ������
    private List<Vector2Int> TryToFitProp(
        Prop prop,
        List<Vector2Int> availablePositions,
        Vector2Int originPosition,
        PlacementOriginCorner placement)
    {
        List<Vector2Int> freePositions = new();

        //��������� ������������ ���� � ����������� �� PlacementOriginCorner
        if (placement == PlacementOriginCorner.BottomLeft)
        {
            for (int xOffset = 0; xOffset < prop.PropSize.x; xOffset++)
            {
                for (int yOffset = 0; yOffset < prop.PropSize.y; yOffset++)
                {
                    Vector2Int tempPos = originPosition + new Vector2Int(xOffset, yOffset);
                    if (availablePositions.Contains(tempPos))
                        freePositions.Add(tempPos);
                }
            }
        }
        else if (placement == PlacementOriginCorner.BottomRight)
        {
            for (int xOffset = -prop.PropSize.x + 1; xOffset <= 0; xOffset++)
            {
                for (int yOffset = 0; yOffset < prop.PropSize.y; yOffset++)
                {
                    Vector2Int tempPos = originPosition + new Vector2Int(xOffset, yOffset);
                    if (availablePositions.Contains(tempPos))
                        freePositions.Add(tempPos);
                }
            }
        }
        else if (placement == PlacementOriginCorner.TopLeft)
        {
            for (int xOffset = 0; xOffset < prop.PropSize.x; xOffset++)
            {
                for (int yOffset = -prop.PropSize.y + 1; yOffset <= 0; yOffset++)
                {
                    Vector2Int tempPos = originPosition + new Vector2Int(xOffset, yOffset);
                    if (availablePositions.Contains(tempPos))
                        freePositions.Add(tempPos);
                }
            }
        }
        else
        {
            for (int xOffset = -prop.PropSize.x + 1; xOffset <= 0; xOffset++)
            {
                for (int yOffset = -prop.PropSize.y + 1; yOffset <= 0; yOffset++)
                {
                    Vector2Int tempPos = originPosition + new Vector2Int(xOffset, yOffset);
                    if (availablePositions.Contains(tempPos))
                        freePositions.Add(tempPos);
                }
            }
        }

        return freePositions;
    }

    // ����������� ������� � ����� �������
    private void PlaceCornerProps(Room room, List<Prop> cornerProps)
    {
        float tempChance = cornerPropPlacementChance; // ���� ����������

        foreach (Vector2Int cornerTile in room.CornerTiles)
        {
            if (UnityEngine.Random.value < tempChance)
            {
                Prop propToPlace = cornerProps[UnityEngine.Random.Range(0, cornerProps.Count)];
                // �������� �������� ������� ������

                PlacePropGameObjectAt(room, cornerTile, propToPlace); // ��������� ���
                if (propToPlace.PlaceAsGroup)
                {
                    PlaceGroupObject(room, cornerTile, propToPlace, 2); // ���� ������ ����� ���� �������� ��� ������
                }
            }
            else
            {
                tempChance = Mathf.Clamp01(tempChance + 0.1f);
            }
        }
    }

    // �������� ����� ��������� ����� ������ ��������������������������� ��� ���������� ��������� � ������
    // searchOffset - �������� ������ ex 1 = �� �������� ��� ������, ����������� �� ���������� 1 ������� �� �������� �������
    private void PlaceGroupObject(Room room, Vector2Int groupOriginPosition, Prop propToPlace, int searchOffset)
    {
        //*����� �������� � �������� ������� ��������(

        //������� ���������� �������� � ������ 
        int count = UnityEngine.Random.Range(propToPlace.GroupMinCount, propToPlace.GroupMaxCount) - 1;
        count = Mathf.Clamp(count, 0, 8);

        //����� ��������� ����� ������ ����������� �����.
        //���������� �������� ������, ����� ���������� ���������� ����� ����� ������� � ����������� ������
        List<Vector2Int> availableSpaces = new List<Vector2Int>();
        for (int xOffset = -searchOffset; xOffset <= searchOffset; xOffset++)
        {
            for (int yOffset = -searchOffset; yOffset <= searchOffset; yOffset++)
            {
                Vector2Int tempPos = groupOriginPosition + new Vector2Int(xOffset, yOffset);
                if (room.FloorTiles.Contains(tempPos) &&
                    !dungeonData.Path.Contains(tempPos) &&
                    !room.PropPositions.Contains(tempPos))
                {
                    availableSpaces.Add(tempPos);
                }
            }
        }

        //�������� ��� �������
        availableSpaces.OrderBy(x => Guid.NewGuid());

        //��������� ������� (�������, ������� �� �����, ���, ���� ����� ������, ��������� ��� ��������� �����)
        int tempCount = count < availableSpaces.Count ? count : availableSpaces.Count;
        for (int i = 0; i < tempCount; i++)
        {
            PlacePropGameObjectAt(room, availableSpaces[i], propToPlace);
            // ��������� ������
        }

    }


    // �������� ������� ��� ����� ������ GameObject � ��������� �������
    private GameObject PlacePropGameObjectAt(Room room, Vector2Int placementPostion, Prop propToPlace)
    {
        

        //���������� ������ ��� ������ 
        if (propToPlace.PropSprite != null)
        {
            //������� ������� � ���� �������
            GameObject prop = Instantiate(propPrefab);
            prop.transform.localPosition = (Vector2)placementPostion;

            SpriteRenderer propSpriteRenderer = prop.GetComponentInChildren<SpriteRenderer>();
            //���������� ������
            propSpriteRenderer.sprite = propToPlace.PropSprite;
            //�������� ���������
            propSpriteRenderer.gameObject.AddComponent<PolygonCollider2D>();

            //�������������� ��������� ������� � ������������ �� ��������
            propSpriteRenderer.transform.localPosition = (Vector2)propToPlace.PropSize * 0.5f;

            //��������� ����� � ������ ������� 
            room.PropObjectReferences.Add(prop);
            room.PropPositions.Add(placementPostion);
            return prop;
        }
        else if(propToPlace.PropPrefabs!= null)
        {
            GameObject PropPrefab;
            if (propToPlace.PropPrefabs.Length > 1)
            {
                PropPrefab = Instantiate(propToPlace.PropPrefabs[Random.Range(0, propToPlace.PropPrefabs.Length)]);
            }else
                PropPrefab = Instantiate(propToPlace.PropPrefabs[0]);

            PropPrefab.transform.localPosition = (Vector2)placementPostion;
            SpriteRenderer propSpriteRenderer = PropPrefab.GetComponentInChildren<SpriteRenderer>();
            propSpriteRenderer.transform.localPosition = (Vector2)propToPlace.PropSize * 0.5f;


            //��������� ����� � ������ ������� 
            room.PropObjectReferences.Add(PropPrefab);
            room.PropPositions.Add(placementPostion);
            return PropPrefab;
        }
        else
        {
            Debug.LogError("Can't find sprite or prefab of prop!");
            return null;
        }            
        
    }
}


//��� ������ ���������� ���������, ��������, ������ � ������ ������� ���� � ������ ���� �� ���������
//������������ ������ � ������ � ������ ���������� ������ �������� �������

public enum PlacementOriginCorner
{
    BottomLeft,
    BottomRight,
    TopLeft,
    TopRight
}
