using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowingScript : MonoBehaviour 
{
    public GameObject target;

    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    void LateUpdate()
    {
        transform.position = target.transform.position + offset;
    }
}
