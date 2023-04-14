using TMPro.EditorUtilities;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // The map cell array and map size classification.
    public MapCell[,] mapCells;
    public int mapSizeX;
    public int mapSizeY;
    public int mapLength;

    // Available prefabs for generation, and the GameObject they are stored in.
    public GameObject mapCellContainer;
    public AvailableCellsScriptableObject availableCells;

    // Fields for evaluation during generation.
    private MapCell currentCell;
    private MapCell previousCell;
    private MapCell startCell;
    private MapCell endCell;
    private DirectionOption departureDirection;
    private int cellDirectionOptionCount = System.Enum.GetNames(typeof(DirectionOption)).Length;

    // Map coordinates used in generation.
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

    

    private void Awake()
    {
        mapCells = new MapCell[mapSizeX, mapSizeY];
        GenerateDungeonPath();
    }

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

        endCell = mapCells[cellX, cellY] = Instantiate(availableCells.endCell);
        endCell.transform.parent = mapCellContainer.transform;
        endCell.transform.position = cellPosition;
        endCell.name = "EndCell";
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
}

public enum DirectionOption
{
    North,
    South,
    East,
    West
}