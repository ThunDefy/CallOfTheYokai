using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PropPlacementManager : MonoBehaviour
{
    MapData dungeonData;

    [SerializeField]
    private List<Prop> propsToPlace;

    [SerializeField, Range(0, 1)]
    private float cornerPropPlacementChance = 0.7f; // ���� ���������� ������� � ����

    [SerializeField]
    private GameObject propPrefab;

    public UnityEvent OnFinished;

    private void Awake()
    {
        dungeonData = FindObjectOfType<MapData>();
    }

    public void ProcessRooms()
    {
        if (dungeonData == null)
            return;

        foreach (Room room in dungeonData.Rooms)
        {
            //���������� ������� �� �����
            List<Prop> cornerProps = propsToPlace.Where(x => x.Corner).ToList();
            PlaceCornerProps(room, cornerProps);

            //����� ����� �����
            List<Prop> leftWallProps = propsToPlace.Where(x => x.NearWallLeft).OrderByDescending(x => x.PropSize.x * x.PropSize.y).ToList();

            PlaceProps(room, leftWallProps, room.NearWallTilesLeft, PlacementOriginCorner.BottomLeft);
            //����� ������ �����
            List<Prop> rightWallProps = propsToPlace.Where(x => x.NearWallRight).OrderByDescending(x => x.PropSize.x * x.PropSize.y).ToList();
            PlaceProps(room, rightWallProps, room.NearWallTilesRight, PlacementOriginCorner.TopRight);

            //����� ������� �����
            List<Prop> topWallProps = propsToPlace.Where(x => x.NearWallUP).OrderByDescending(x => x.PropSize.x * x.PropSize.y).ToList();
            PlaceProps(room, topWallProps, room.NearWallTilesUp, PlacementOriginCorner.TopLeft);

            //����� ������ �����
            List<Prop> downWallProps = propsToPlace.Where(x => x.NearWallDown).OrderByDescending(x => x.PropSize.x * x.PropSize.y).ToList();
            PlaceProps(room, downWallProps, room.NearWallTilesDown, PlacementOriginCorner.BottomLeft);

            //������ �������
            List<Prop> innerProps = propsToPlace.Where(x => x.Inner).OrderByDescending(x => x.PropSize.x * x.PropSize.y).ToList();
            PlaceProps(room, innerProps, room.InnerTiles, PlacementOriginCorner.BottomLeft);
        }

        Invoke("RunEvent", 1);

    }

    public void RunEvent()
    {
        OnFinished?.Invoke();
    }


    // ��������� ������� ����� ����. ��� ����� ������� �������� � ����� ������ ����������
    private void PlaceProps(
        Room room, List<Prop> wallProps, HashSet<Vector2Int> availableTiles, PlacementOriginCorner placement)
    {
        //������� ������� ���� �� ��������� ������ nearWallTiles, ����� ���������� ��������� ���� ��� ����������� ����������
        HashSet<Vector2Int> tempPositons = new HashSet<Vector2Int>(availableTiles);
        tempPositons.ExceptWith(dungeonData.Path);

        //����������� ���������� ��� �������
        foreach (Prop propToPlace in wallProps)
        {
            //����� ���������� ������ ������������ ���������� ������� �������
            int quantity = UnityEngine.Random.Range(propToPlace.PlacementQuantityMin, propToPlace.PlacementQuantityMax + 1);

            for (int i = 0; i < quantity; i++)
            {
                //������� ������� �������
                tempPositons.ExceptWith(room.PropPositions);
                //������������ �������
                List<Vector2Int> availablePositions = tempPositons.OrderBy(x => Guid.NewGuid()).ToList();
                //���� ���������� �� �������, ��� ������ �������� ���������� ��� �� ����� �������� �����
                if (TryPlacingPropBruteForce(room, propToPlace, availablePositions, placement) == false)
                    break;
            }

        }
    }


    // �������� ���������� ������, ��������� ������ ���� (������ ������ ��������� ������� ������).
    private bool TryPlacingPropBruteForce(
        Room room, Prop propToPlace, List<Vector2Int> availablePositions, PlacementOriginCorner placement)
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
    private void PlaceGroupObject(
        Room room, Vector2Int groupOriginPosition, Prop propToPlace, int searchOffset)
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
        //������� ������� � ���� �������
        GameObject prop = Instantiate(propPrefab);
        SpriteRenderer propSpriteRenderer = prop.GetComponentInChildren<SpriteRenderer>();

        //���������� ������
        propSpriteRenderer.sprite = propToPlace.PropSprite;

        //�������� ���������
        CapsuleCollider2D collider
            = propSpriteRenderer.gameObject.AddComponent<CapsuleCollider2D>();
        collider.offset = Vector2.zero;
        if (propToPlace.PropSize.x > propToPlace.PropSize.y)
        {
            collider.direction = CapsuleDirection2D.Horizontal;
        }
        Vector2 size
            = new Vector2(propToPlace.PropSize.x * 0.8f, propToPlace.PropSize.y * 0.8f);
        collider.size = size;

        prop.transform.localPosition = (Vector2)placementPostion;

        //�������������� ��������� ������� � ������������ �� ��������
        propSpriteRenderer.transform.localPosition = (Vector2)propToPlace.PropSize * 0.5f;

        //��������� ����� � ������ ������� 
        room.PropPositions.Add(placementPostion);
        room.PropObjectReferences.Add(prop);
        return prop;
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
