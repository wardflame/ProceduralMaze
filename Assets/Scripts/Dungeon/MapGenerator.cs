using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public MapCell currentCell;
    public MapCell previousCell;
    public MapCell startCell;
    public MapCell endCell;
    private DirectionOption departureDirection;
    private int cellDirectionOptionCount = System.Enum.GetNames(typeof(DirectionOption)).Length;
    private int posX = 0;
    private int posY = 0;
    private int PosX
    {
        get { return posX; }
        set 
        {
            if (value >= 0 && value < mapSizeX) posX = value;
        }
    }
    private int PosY
    {
        get { return posY; }
        set
        {
            int previousY = posY;
            if (value >= 0 && value < mapSizeY) posY = value;
        }
    }

    public MapCell[,] mapCells;
    public GameObject mapCellContainer;
    public AvailableCellsScriptableObject availableCells;
    public int mapSizeX;
    public int mapSizeY;
    public int mapLength;

    private void Awake()
    {
        mapCells = new MapCell[mapSizeX, mapSizeY];
        GenerateDungeonPath();
    }

    private void GenerateDungeonPath()
    {
        GenerateStartCell();
        GenerateRandomCells();
    }

    private void GenerateStartCell()
    {
        PosX = Random.Range(0, mapSizeX);
        PosY = Random.Range(0, mapSizeY);

        mapCells[PosX, PosY] = Instantiate(availableCells.startCell);

        startCell = mapCells[PosX, PosY];
        startCell.transform.parent = mapCellContainer.transform;

        currentCell = mapCells[PosX, PosY];
        currentCell.mapX = PosX;
        currentCell.mapY = PosY;
        previousCell = mapCells[PosX, PosY];
    }

    private bool NeighbourValidationCheck(MapCell targetCell)
    {
        if (targetCell == previousCell) return false;
        if (targetCell != null && targetCell.pathfinderVisited) return false;
        return true;
    }

    private void FindNeighbourCell()
    {
        var randomDirection = (DirectionOption)Random.Range(0, cellDirectionOptionCount);

        if (randomDirection == DirectionOption.North) PosY++;
        else if (randomDirection == DirectionOption.South) PosY--;
        else if (randomDirection == DirectionOption.East) PosX++;
        else if (randomDirection == DirectionOption.West) PosX--;

        var targetCell = mapCells[PosX, PosY];

        if (!NeighbourValidationCheck(targetCell))
        {
            PosX = previousCell.mapX;
            PosY = previousCell.mapY;
            FindNeighbourCell();
        }
        departureDirection = randomDirection;
    }

    private void SpawnCell()
    {
        int randomCellIndex = Random.Range(0, availableCells.availableRandomCells.Length);
        mapCells[PosX, PosY] = Instantiate(availableCells.availableRandomCells[randomCellIndex]);
        currentCell = mapCells[PosX, PosY];

        Vector3 targetPosition = previousCell.transform.position;
        if (departureDirection == DirectionOption.North)
        {
            targetPosition.z += 10;
            currentCell.wallSouth.SetActive(false);
            previousCell.wallNorth.SetActive(false);
        }
        else if (departureDirection == DirectionOption.South)
        {
            targetPosition.z -= 10;
            currentCell.wallNorth.SetActive(false);
            previousCell.wallSouth.SetActive(false);
        }
        else if (departureDirection == DirectionOption.East)
        {
            targetPosition.x += 10;
            currentCell.wallWest.SetActive(false);
            previousCell.wallEast.SetActive(false);
        }
        else if (departureDirection == DirectionOption.West)
        {
            targetPosition.x -= 10;
            currentCell.wallEast.SetActive(false);
            previousCell.wallWest.SetActive(false);
        }

        currentCell.transform.position = targetPosition;
        currentCell.transform.parent = mapCellContainer.transform;
        currentCell.pathfinderVisited = true;
    }

    private void GenerateRandomCells()
    {
        for (int i = 0; i < mapLength; i++)
        {
            previousCell = mapCells[PosX, PosY];
            FindNeighbourCell();
            SpawnCell();
        }
    }

    private void AssignEndCell()
    {
        MapCell lastCell = mapCellContainer.transform.GetChild(mapCellContainer.transform.childCount - 1).GetComponent<MapCell>();
        Destroy(lastCell);
        lastCell = Instantiate(availableCells.endCell);
        endCell = lastCell;
        endCell.transform.parent = mapCellContainer.transform;
        mapCells[PosX, PosY] = endCell;
    }
}

public enum DirectionOption
{
    North,
    South,
    East,
    West
}