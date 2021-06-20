using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonkeMove : MonoBehaviour
{
    public Transform bananaYumYum;

    public float moveSpeed = 5f;

    public Tilemap groundTilemap;
    public Tilemap levelTilemap;

    private Vector3 targetPosition;
    private Rigidbody2D rb;

    // To find a spot on or around banana
    private static int[] gridYOffsets = new int[] { -1, -1, -1,   0, 0, 0,   1, 1, 1 };
    private static int[] gridXOffsets = new int[] { -1,  0,  1,  -1, 0, 1,  -1, 0, 1 };

    void Start()
    {
        targetPosition = FindPositionAroundBanana();
        rb = GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
            targetPosition = FindPositionAroundBanana();
    }

    void FixedUpdate()
    {
        Vector3 move = Vector3.MoveTowards(rb.position, targetPosition, moveSpeed * Time.deltaTime);
        rb.MovePosition(move);
    }

    Vector3 FindPositionAroundBanana()
    {
        Vector3Int bananaGridPosition = groundTilemap.WorldToCell(bananaYumYum.position);
        Vector3 position = (Vector3) bananaGridPosition;

        int offsetIndex = Random.Range(0, gridYOffsets.Length);
        for (int i = offsetIndex; i != offsetIndex - 1; i = (i + 1) % gridYOffsets.Length)
        {
            Vector3Int positionToCheck = bananaGridPosition + new Vector3Int(gridXOffsets[i], gridYOffsets[i], 0);
            if (groundTilemap.HasTile(positionToCheck) && !levelTilemap.HasTile(positionToCheck))
            {
                position = (Vector3) positionToCheck;
                break;
            }
        }

        position.x += 0.5f;
        position.y += 0.5f;

        return position;
    }
}
