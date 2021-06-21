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

    private List<Vector3> gridPath;
    private int currentPathIndex;

    // To find a spot on or around banana
    private static int[] gridYOffsets = new int[] { -1, -1, -1,   0, 0, 0,   1, 1, 1 };
    private static int[] gridXOffsets = new int[] { -1,  0,  1,  -1, 0, 1,  -1, 0, 1 };

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        gridPath = new List<Vector3>();
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

        Vector3 move = Vector3.MoveTowards(rb.position, gridPath[currentPathIndex], settings.moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(move);

        if (((Vector3) rb.position - gridPath[currentPathIndex]).sqrMagnitude < 0.01f)
            currentPathIndex++;
    }

    Vector3Int FindPositionAroundBanana()
    {
        Vector3Int bananaGridPosition = groundTilemap.WorldToCell(bananaYumYum.position);

        int offsetIndex = Random.Range(0, gridYOffsets.Length);

        {   // Swap X Offsets
            int temp = gridXOffsets[0];
            gridXOffsets[0] = gridXOffsets[offsetIndex];
            gridXOffsets[offsetIndex] = temp;
        }

        {   // Swap Y Offsets
            int temp = gridYOffsets[0];
            gridYOffsets[0] = gridYOffsets[offsetIndex];
            gridYOffsets[offsetIndex] = temp;
        }

        for (int i = 0; i != gridYOffsets.Length; i++)
        {
            Vector3Int positionToCheck = bananaGridPosition + new Vector3Int(gridXOffsets[i], gridYOffsets[i], 0);
            if (groundTilemap.HasTile(positionToCheck) && !levelTilemap.HasTile(positionToCheck))
                return positionToCheck;
        }

        return bananaGridPosition;
    }
}
