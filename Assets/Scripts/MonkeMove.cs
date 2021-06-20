using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// @Watch: https://www.youtube.com/watch?v=CTf0WjhfBx8 for proper top-down sorting

public class MonkeMove : MonoBehaviour
{
    public Transform bananaYumYum;
    public MonkeSettings settings;
    public PathFinder finder;

    [Header("Level")]
    public Tilemap groundTilemap;
    public Tilemap levelTilemap;

    [Header("For Debugging REMOVE LATER")]
    public GameObject circle;
    public List<GameObject> dots = new List<GameObject>();

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
        if (finder.UpdatePath(startPos, FindPositionAroundBanana(), ref gridPath))
        {   // Visual Debugging
            for (int i = 0; i  < gridPath.Count; i++)
                dots.Add(Instantiate(circle, (Vector3) gridPath[i] + new Vector3(0.5f, 0.5f, 0f), Quaternion.identity));
        }
    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int startPos = groundTilemap.WorldToCell(rb.position);
            Vector3Int endPos = FindPositionAroundBanana();
            if (finder.UpdatePath(startPos, endPos, ref gridPath))
            {
                currentPathIndex = 0;

                {   // Visual Debugging
                    for (int i = 0; i  < dots.Count; i++)
                        Destroy(dots[i]);
                    dots.Clear();

                    for (int i = 0; i  < gridPath.Count; i++)
                        dots.Add(Instantiate(circle, (Vector3) gridPath[i] + new Vector3(0.5f, 0.5f, 0f), Quaternion.identity));
                }
            }
        }
    }

    void FixedUpdate()
    {
        Debug.Log("Grid Path Count: " + gridPath.Count);
        if (currentPathIndex >= gridPath.Count)
            return;

        Vector3 targetPosition = dots[currentPathIndex].transform.position;
        Vector3 move = Vector3.MoveTowards(rb.position, targetPosition, settings.moveSpeed * Time.deltaTime);
        rb.MovePosition(move);

        if (((Vector3) rb.position - targetPosition).sqrMagnitude < 0.01f)
        {
            dots[currentPathIndex].active = false;
            currentPathIndex++;
        }
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
