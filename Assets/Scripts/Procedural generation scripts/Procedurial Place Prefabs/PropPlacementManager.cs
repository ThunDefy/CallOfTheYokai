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
    private float cornerPropPlacementChance = 0.7f; // Шанс размещения объекта в углу

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
            //Расставить объекты по углам
            List<Prop> cornerProps = propsToPlace.Where(x => x.Corner).ToList();
            PlaceCornerProps(room, cornerProps);

            //Возле левой стены
            List<Prop> leftWallProps = propsToPlace.Where(x => x.NearWallLeft).OrderByDescending(x => x.PropSize.x * x.PropSize.y).ToList();

            PlaceProps(room, leftWallProps, room.NearWallTilesLeft, PlacementOriginCorner.BottomLeft);
            //Возле правой стены
            List<Prop> rightWallProps = propsToPlace.Where(x => x.NearWallRight).OrderByDescending(x => x.PropSize.x * x.PropSize.y).ToList();
            PlaceProps(room, rightWallProps, room.NearWallTilesRight, PlacementOriginCorner.TopRight);

            //Возле верхней стены
            List<Prop> topWallProps = propsToPlace.Where(x => x.NearWallUP).OrderByDescending(x => x.PropSize.x * x.PropSize.y).ToList();
            PlaceProps(room, topWallProps, room.NearWallTilesUp, PlacementOriginCorner.TopLeft);

            //Возле нижней стены
            List<Prop> downWallProps = propsToPlace.Where(x => x.NearWallDown).OrderByDescending(x => x.PropSize.x * x.PropSize.y).ToList();
            PlaceProps(room, downWallProps, room.NearWallTilesDown, PlacementOriginCorner.BottomLeft);

            //Внутри комнаты
            List<Prop> innerProps = propsToPlace.Where(x => x.Inner).OrderByDescending(x => x.PropSize.x * x.PropSize.y).ToList();
            PlaceProps(room, innerProps, room.InnerTiles, PlacementOriginCorner.BottomLeft);
        }

        //OnFinished?.Invoke();
        Invoke("RunEvent", 1);

    }

    public void RunEvent()
    {
        OnFinished?.Invoke();
    }

    //private IEnumerator TutorialCoroutine(Action code)
    //{
    //    yield return new WaitForSeconds(3);
    //    code();
    //}

    // Размещает объекты возле стен. Нам нужно указать реквизит и точку начала размещения
    private void PlaceProps(
        Room room, List<Prop> wallProps, HashSet<Vector2Int> availableTiles, PlacementOriginCorner placement)
    {
        //Удалить позиции пути из начальных плиток nearWallTiles, чтобы обеспечить свободный путь для прохождения подземелья
        HashSet<Vector2Int> tempPositons = new HashSet<Vector2Int>(availableTiles);
        tempPositons.ExceptWith(dungeonData.Path);

        //постараемся разместить все объекты
        foreach (Prop propToPlace in wallProps)
        {
            //хотим разместить только определенное количество каждого объекта
            int quantity = UnityEngine.Random.Range(propToPlace.PlacementQuantityMin, propToPlace.PlacementQuantityMax + 1);

            for (int i = 0; i < quantity; i++)
            {
                //удалить занятые позиции
                tempPositons.ExceptWith(room.PropPositions);
                //перетасовать позиции
                List<Vector2Int> availablePositions = tempPositons.OrderBy(x => Guid.NewGuid()).ToList();
                //Если размещение не удалось, нет смысла пытаться разместить тот же самый реквизит снова
                if (TryPlacingPropBruteForce(room, propToPlace, availablePositions, placement) == false)
                    break;
            }

        }
    }


    // Пытается разместить объект, используя грубую силу (пробуя каждую доступную позицию плитки).
    private bool TryPlacingPropBruteForce(
        Room room, Prop propToPlace, List<Vector2Int> availablePositions, PlacementOriginCorner placement)
    {
        //попробуем разместить объекты, начиная с угла, указанного параметром placement
        for (int i = 0; i < availablePositions.Count; i++)
        {
            //выберать указанную позицию (но она может быть уже занята после размещения угловых реквизитов в виде группы)
            Vector2Int position = availablePositions[i];
            if (room.PropPositions.Contains(position))
                continue;

            //проверить, достаточно ли места вокруг для установки объекта
            List<Vector2Int> freePositionsAround
                = TryToFitProp(propToPlace, availablePositions, position, placement);

            //Если у нас будет достаточно места, разместить объект
            if (freePositionsAround.Count == propToPlace.PropSize.x * propToPlace.PropSize.y)
            {
                //Поместите игровой объект
                PlacePropGameObjectAt(room, position, propToPlace);
                //Зафиксироватье все позиции, требуемые объектом (в зависимости от его размера).
                foreach (Vector2Int pos in freePositionsAround)
                {
                    //Hashest будет игнорировать дублирующие позиции
                    room.PropPositions.Add(pos);
                }

                //Разберемся с группами объектов
                if (propToPlace.PlaceAsGroup)
                {
                    PlaceGroupObject(room, position, propToPlace, 1);
                }
                return true;
            }
        }

        return false;
    }

    // Проверка, подойдет ли объект
    private List<Vector2Int> TryToFitProp(
        Prop prop,
        List<Vector2Int> availablePositions,
        Vector2Int originPosition,
        PlacementOriginCorner placement)
    {
        List<Vector2Int> freePositions = new();

        //Выполнить определенный цикл в зависимости от PlacementOriginCorner
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

    // Расставляет объекты в углах комнаты
    private void PlaceCornerProps(Room room, List<Prop> cornerProps)
    {
        float tempChance = cornerPropPlacementChance; // шанс размещения

        foreach (Vector2Int cornerTile in room.CornerTiles)
        {
            if (UnityEngine.Random.value < tempChance)
            {
                Prop propToPlace = cornerProps[UnityEngine.Random.Range(0, cornerProps.Count)];
                // случайно выбираем угловой объект

                PlacePropGameObjectAt(room, cornerTile, propToPlace); // размещаем его
                if (propToPlace.PlaceAsGroup)
                {
                    PlaceGroupObject(room, cornerTile, propToPlace, 2); // Если объект хочет быть размещен как группа
                }
            }
            else
            {
                tempChance = Mathf.Clamp01(tempChance + 0.1f);
            }
        }
    }

    // Помогает найти свободные места вокруг группыОригинальноеПоложение для размещения реквизита в группе
    // searchOffset - Смещение поиска ex 1 = мы проверим все плитки, находящиеся на расстоянии 1 единицы от исходной позиции
    private void PlaceGroupObject(
        Room room, Vector2Int groupOriginPosition, Prop propToPlace, int searchOffset)
    {
        //*Плохо работает с группами больших объектов(

        //Подсчет количества объектов в группе 
        int count = UnityEngine.Random.Range(propToPlace.GroupMinCount, propToPlace.GroupMaxCount) - 1;
        count = Mathf.Clamp(count, 0, 8);

        //найти свободные места вокруг центральной точки.
        //используем смещение поиска, чтобы ограничить расстояние между этими точками и центральной точкой
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

        //РАндомим эти позиции
        availableSpaces.OrderBy(x => Guid.NewGuid());

        //размещаем объекты (столько, сколько мы хотим, или, если места меньше, заполните все доступные места)
        int tempCount = count < availableSpaces.Count ? count : availableSpaces.Count;
        for (int i = 0; i < tempCount; i++)
        {
            PlacePropGameObjectAt(room, availableSpaces[i], propToPlace);
            // Размещаем объект
        }

    }


    // Помещает объекты как новый объект GameObject в указанную позицию
    private GameObject PlacePropGameObjectAt(Room room, Vector2Int placementPostion, Prop propToPlace)
    {
        //Создать элемент в этой позиции
        GameObject prop = Instantiate(propPrefab);
        SpriteRenderer propSpriteRenderer = prop.GetComponentInChildren<SpriteRenderer>();

        //Установить спрайт
        propSpriteRenderer.sprite = propToPlace.PropSprite;

        //Добавить коллайдер
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

        //Отрегулировать положение объекта в соответствии со спрайтом
        propSpriteRenderer.transform.localPosition = (Vector2)propToPlace.PropSize * 0.5f;

        //Сохраните оъект в данных комнаты 
        room.PropPositions.Add(placementPostion);
        room.PropObjectReferences.Add(prop);
        return prop;
    }
}


//Где начать размещение реквизита, например, начать с левого нижнего угла и искать есть ли свободное
//пространство справа и сверху в случае размещения стойки большого размера

public enum PlacementOriginCorner
{
    BottomLeft,
    BottomRight,
    TopLeft,
    TopRight
}
