using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeWinCondition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerStatus>().InvokeWin();
        }
    }
}
