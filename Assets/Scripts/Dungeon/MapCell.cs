using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCell : MonoBehaviour
{
    public int mapX;
    public int mapY;
    public bool playerExplored;
    public GameObject floor;
    public GameObject wallNorth;
    public GameObject wallEast;
    public GameObject wallWest;
    public GameObject wallSouth;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") playerExplored = true;
    }
}
