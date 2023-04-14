using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 velocity = Vector3.zero;

    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 10f;

    private void FixedUpdate()
    {
        Vector3 followPosition = target.position + offset;
        Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, followPosition, ref velocity, smoothSpeed * Time.deltaTime);
        transform.position = smoothPosition;
    }
}
