using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonkeMove : MonoBehaviour
{
    public Transform bananaYumYum;
    public MonkeSettings settings;
    public PathFinder finder;

    [Header("Level")]
    public Tilemap groundTilemap;
    public Tilemap levelTilemap;

    private Rigidbody2D rb;

    private List<Vector3Int> gridPath;
    private int currentPathIndex;

    // To find a spot on or around banana
    private static int[] gridYOffsets = new int[] { -1, -1, -1,   0, 0, 0,   1, 1, 1 };
    private static int[] gridXOffsets = new int[] { -1,  0,  1,  -1, 0, 1,  -1, 0, 1 };

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        gridPath = new List<Vector3Int>();
        currentPathIndex = 0;

        Vector3Int startPos = groundTilemap.WorldToCell(rb.position);
        finder.UpdatePath(startPos, FindPositionAroundBanana(), ref gridPath);
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int startPos = groundTilemap.WorldToCell(rb.position);
            Vector3Int endPos = FindPositionAroundBanana();
            finder.UpdatePath(startPos, endPos, ref gridPath);
            currentPathIndex = 0;
        }
    }

    void FixedUpdate()
    {
        if (currentPathIndex >= gridPath.Count)
            return;

        Vector3 targetPosition = (Vector3) gridPath[currentPathIndex] + new Vector3(0.5f, 0.5f, 0f);
        Vector3 move = Vector3.MoveTowards(rb.position, targetPosition, settings.moveSpeed * Time.deltaTime);
        rb.MovePosition(move);

        if (((Vector3) rb.position - targetPosition).sqrMagnitude < 0.01f)
            currentPathIndex++;
    }

    // @Todo: Change this to return Vector3Int for pathfinding
    Vector3Int FindPositionAroundBanana()
    {
        Vector3Int bananaGridPosition = groundTilemap.WorldToCell(bananaYumYum.position);

        int offsetIndex = Random.Range(0, gridYOffsets.Length);
        for (int i = offsetIndex; i != offsetIndex - 1; i = (i + 1) % gridYOffsets.Length)
        {
            Vector3Int positionToCheck = bananaGridPosition + new Vector3Int(gridXOffsets[i], gridYOffsets[i], 0);
            if (groundTilemap.HasTile(positionToCheck) && !levelTilemap.HasTile(positionToCheck))
                return positionToCheck;
        }

        return bananaGridPosition;
    }
}
