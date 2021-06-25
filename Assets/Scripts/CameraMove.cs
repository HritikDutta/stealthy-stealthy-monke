using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed = 10f;
    public Transform target;

    void Update()
    {
        // @Todo: This should be a 2 way lerp (It should be smooth at the beginning and the end).
        Vector3 interpPosition = Vector3.Lerp(transform.position, target.position, moveSpeed * Time.deltaTime);
        transform.position = interpPosition;
    }
}
