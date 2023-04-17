using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    #region Map fields
    public MapCell[,] mapCells;
    public int mapSizeX;
    public int mapSizeY;
    public int mapLength;
    #endregion

    #region Available cell prefabs and container object
    public GameObject mapCellContainer;
    public AvailableCellsScriptableObject availableCells;
    #endregion

    #region Generation fields
    private MapCell currentCell;
    private MapCell previousCell;
    private MapCell startCell;
    private MapCell endCell;
    private DirectionOption departureDirection;
    private int cellDirectionOptionCount = System.Enum.GetNames(typeof(DirectionOption)).Length;
    #endregion

    #region Map coordinates
    private int positionX = 0;
    private int positionY = 0;
    private int PosX
    {
        get { return positionX; }
        set
        {
            if (value >= 0 && value < mapSizeX) positionX = value;
        }
    }
    private int PosY
    {
        get { return positionY; }
        set
        {
            if (value >= 0 && value < mapSizeY) positionY = value;
        }
    }
    #endregion

    private void Awake()
    {
        mapCells = new MapCell[mapSizeX, mapSizeY];
        GenerateDungeonPath();
    }

    #region Generation methods
    private void GenerateDungeonPath()
    {
        GenerateStartCell();
        for (int i = 0; i < mapLength - 1; i++) // Deduct 1 as start cell is generated prior to loop.
        {
            previousCell = mapCells[PosX, PosY];
            if (!FindNeighbourCell())
            {
                i--;
                continue;
            }
            SpawnCell();
            currentCell.name = $"Cell{i}";
        }
        GenerateEndCell();
    }

    private void GenerateStartCell()
    {
        PosX = Random.Range(0, mapSizeX);
        PosY = Random.Range(0, mapSizeY);

        currentCell = mapCells[PosX, PosY] = Instantiate(availableCells.startCell);
        currentCell.mapX = PosX;
        currentCell.mapY = PosY;
        currentCell.transform.parent = mapCellContainer.transform;
        currentCell.name = "StartCell";
        startCell = mapCells[PosX, PosY];
    }

    private void GenerateEndCell()
    {
        var cellToDestroy = LocateFurthestCell(startCell);
        int cellX = cellToDestroy.mapX;
        int cellY = cellToDestroy.mapY;
        Vector3 cellPosition = cellToDestroy.transform.position;
        Destroy(cellToDestroy.gameObject);

        PosX = cellX;
        PosY = cellY;

        endCell = mapCells[cellX, cellY] = Instantiate(availableCells.endCell);
        endCell.mapX = cellX;
        endCell.mapY = cellY;
        endCell.transform.parent = mapCellContainer.transform;
        endCell.transform.position = cellPosition;
        endCell.name = "EndCell";

        var neighbours = GetNeighbours(endCell);
        foreach (var cell in neighbours)
        {
            if (cell != null)
            {
                if (cell.mapX != cellX)
                {
                    if (cell.mapX > cellX)
                    {
                        if (cell.wallWest) cell.wallWest.SetActive(false);
                        endCell.wallEast.SetActive(false);
                    }
                    else
                    {
                        if (cell.wallEast) cell.wallEast.SetActive(false);
                        endCell.wallWest.SetActive(false);
                    }
                }
                else if (cell.mapY != cellY)
                {
                    if (cell.mapY > cellY)
                    {
                        if (cell.wallSouth) cell.wallSouth.SetActive(false);
                        endCell.wallNorth.SetActive(false);
                    }
                    else
                    {
                        if (cell.wallNorth) cell.wallNorth.SetActive(false);
                        endCell.wallSouth.SetActive(false);
                    }
                }
            }
        }
    }

    private List<MapCell> GetNeighbours(MapCell origin)
    {
        List<MapCell> cells = new List<MapCell>();
                  
        if (origin.mapX + 1 <= mapCells.GetUpperBound(0)) cells.Add(mapCells[origin.mapX + 1, origin.mapY]);
        if (origin.mapY + 1 <= mapCells.GetUpperBound(1)) cells.Add(mapCells[origin.mapX, origin.mapY + 1]);
        if (origin.mapX - 1 >= 0) cells.Add(mapCells[origin.mapX - 1, origin.mapY]);
        if (origin.mapY - 1 >= 0) cells.Add(mapCells[origin.mapX, origin.mapY - 1]);

        return cells;
    }

    private bool FindNeighbourCell()
    {
        var randomDirection = (DirectionOption)Random.Range(0, cellDirectionOptionCount);

        switch (randomDirection)
        {
            case DirectionOption.North:
                {
                    PosY++;
                }
                break;
            case DirectionOption.South:
                {
                    PosY--;
                }
                break;
            case DirectionOption.East:
                {
                    PosX++;
                }
                break;
            case DirectionOption.West:
                {
                    PosX--;
                }
                break;
        }

        var target = mapCells[PosX, PosY];

        if (target != null)
        {
            return false;
        }

        departureDirection = randomDirection;
        return true;
    }

    private void SpawnCell()
    {
        int randomCellIndex = Random.Range(0, availableCells.randomCells.Length);
        currentCell = mapCells[PosX, PosY] = Instantiate(availableCells.randomCells[randomCellIndex]);
        currentCell.mapX = PosX;
        currentCell.mapY = PosY;
        currentCell.transform.parent = mapCellContainer.transform;

        Vector3 destination = previousCell.transform.position;
        switch (departureDirection)
        {
            case DirectionOption.North:
                {
                    destination.z += 10;

                    currentCell.transform.position = destination;

                    currentCell.wallSouth.SetActive(false);
                    previousCell.wallNorth.SetActive(false);
                }
                break;
            case DirectionOption.South:
                {
                    destination.z -= 10;

                    currentCell.transform.position = destination;

                    currentCell.wallNorth.SetActive(false);
                    previousCell.wallSouth.SetActive(false);
                }
                break;
            case DirectionOption.East:
                {
                    destination.x += 10;

                    currentCell.transform.position = destination;

                    currentCell.wallWest.SetActive(false);
                    previousCell.wallEast.SetActive(false);
                }
                break;
            case DirectionOption.West:
                {
                    destination.x -= 10;

                    currentCell.transform.position = destination;

                    currentCell.wallEast.SetActive(false);
                    previousCell.wallWest.SetActive(false);
                }
                break;
        }
    }

    private MapCell LocateFurthestCell(MapCell originCell)
    {
        Vector3 originPosition = originCell.transform.position;
        MapCell furthestCell = originCell;
        float furthestDistance = 0;
        float targetDistanceFromOrigin;

        foreach (var queryCell in mapCells)
        {
            if (queryCell == null) continue;

            targetDistanceFromOrigin = Vector3.Distance(originPosition, queryCell.transform.position);

            if (targetDistanceFromOrigin > furthestDistance)
            {
                furthestDistance = targetDistanceFromOrigin;
                furthestCell = queryCell;
            }
        }

        return furthestCell;
    }
    #endregion
}

public enum DirectionOption
{
    North,
    South,
    East,
    West
}