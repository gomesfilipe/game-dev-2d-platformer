using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowingScript : MonoBehaviour 
{
    public GameObject target;

    void LateUpdate()
    {
        transform.position = new Vector3(
            target.transform.position.x,
            target.transform.position.y + 2f,
            transform.position.z
        );
    }
}
