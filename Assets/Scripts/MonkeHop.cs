using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeHop : MonoBehaviour
{
    public Transform bananaYumYum;
    public MonkeSettings settings;

    private float verticalVelocity;
    private bool isJumping = false;
    private float lastJumpTime;

    void Start()
    {
        lastJumpTime = Time.time;
    }

    void LateUpdate()
    {
        // Face the banana
        bool facingLeft = bananaYumYum.position.x <= transform.position.x;
        float rotation = facingLeft ? 0 : 180;
        transform.localRotation = Quaternion.Euler(0, rotation, 0);
    }

    void FixedUpdate()
    {
        // Hopping
        if (!isJumping && (Time.time - lastJumpTime) >= 0.15f && Random.Range(0f, 1f) <= settings.hopchance)
        {
            verticalVelocity = settings.hopSpeed;
            isJumping = true;
        }

        if (isJumping)
        {
            float height = transform.localPosition.y;
            height += (verticalVelocity * Time.fixedDeltaTime);

            verticalVelocity -= settings.gravity * Time.fixedDeltaTime;

            if (height <= 0f)
            {
                height = 0f;
                isJumping = false;
                lastJumpTime = Time.time;
            }

            transform.localPosition = new Vector3(0f, height, 0f);
        }
    }
}
