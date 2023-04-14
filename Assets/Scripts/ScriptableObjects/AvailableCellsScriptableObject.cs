using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AvailableCells", menuName = "1543493/Map/AvailableCells")]
public class AvailableCellsScriptableObject : ScriptableObject
{
    public MapCell startCell;
    public MapCell endCell;
    public MapCell[] availableRandomCells;
}
