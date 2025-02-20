using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float FollowSpeed = 5f;
    public Transform Target;
    public Vector3 Offset;
    void Update()
    {
        Vector3 newPos = new Vector3(Target.position.x + Offset.x, Target.position.y + Offset.y, Offset.z);
        transform.position = Vector3.Slerp(transform.position, newPos, FollowSpeed * Time.deltaTime);
    }
}
