using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @Todo: Use something similar to boids I guess
public class MonkeMove : MonoBehaviour
{
    public Transform bananaYumYum;
    public float moveSpeed = 5f;
    public float hopSpeed = 1.5f;
    public float gravity = 10f;
    public float hopchance = 0.75f;
    public float multiplier = 0.04f;

    private Vector3 targetPosition;
    private Vector3 actualPosition;
    private Vector3 offsetPosition;
    private float verticalVelocity;
    private float lastJumpTime;
    private bool isJumping;

    void Start()
    {
        actualPosition = transform.position;
        offsetPosition = Vector3.zero;

        verticalVelocity = 0f;
        isJumping = false;

        lastJumpTime = Time.time;
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(0))
            targetPosition = bananaYumYum.position + new Vector3(3 * multiplier * Random.Range(-1f, 1f), 3 *  multiplier * Random.Range(-1f, 1f), 0f);

        actualPosition = Vector3.MoveTowards(actualPosition, targetPosition, multiplier * moveSpeed * Time.deltaTime);
        bool facingLeft = targetPosition.x >= actualPosition.x;

        if (!isJumping && (Time.time - lastJumpTime) >= 0.15f && Random.Range(0f, 1f) <= hopchance)
        {
            verticalVelocity = hopSpeed;
            isJumping = true;
        }

        if (isJumping)
        {
            offsetPosition.y += verticalVelocity * Time.deltaTime;
            verticalVelocity -= gravity * Time.deltaTime;

            if (transform.position.y + offsetPosition.y <= actualPosition.y)
            {
                offsetPosition.y = 0f;
                isJumping = false;
                lastJumpTime = Time.time;
            }
        }

        transform.position = actualPosition + offsetPosition;

        float rotation = facingLeft ? 180 : 0;
        transform.localRotation = Quaternion.Euler(0, rotation, 0);
    }
}
