using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform targetToFollow;

    public Vector3 cameraOffset;

    public float smoothFactor = 0.5f;

    void FixedUpdate()
    {
        Vector3 newpos = targetToFollow.position + cameraOffset;

        transform.position = Vector3.Slerp(transform.position, newpos, smoothFactor);
    }
}
