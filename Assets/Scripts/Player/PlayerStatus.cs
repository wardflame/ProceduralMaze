using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public delegate void WinConditionCallback();
    public event WinConditionCallback winCallback;

    public void InvokeWin()
    {
        winCallback.Invoke();
    }
}
