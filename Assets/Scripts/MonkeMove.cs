using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum MonkeMood
{
    Idle,
    BananaYumYum,
    OhNoGuard,
    Captured,
}

public enum MonkeRunMode
{
    RunBack,
    Scatter,
}

public class MonkeMove : MonoBehaviour
{
    [Header("Behaviour")]
    public MonkeMood mood;

    [Header("Settings")]
    public MonkeSettings settings;

    [Header("Level")]
    public Tilemap groundTilemap;
    public Tilemap levelTilemap;

    [Header("Visual")]
    public MonkeVisual visual;

    private GameObject gameobject;
    private Rigidbody2D rb;

    private List<Vector3> gridPath;
    private int currentPathIndex;

    private Transform guardTransform;
    private MonkeRunMode runMode;

    // To find a spot on or around banana
    private static int[] gridYOffsets = new int[] { -1, -1, -1,   0, 0, 0,   1, 1, 1 };
    private static int[] gridXOffsets = new int[] { -1,  0,  1,  -1, 0, 1,  -1, 0, 1 };

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gameobject = GetComponent<GameObject>();
        gridPath = new List<Vector3>();
        visual = transform.Find("visual").GetComponent<MonkeVisual>();
    }

    void LateUpdate()
    {
        // If Idle:     // Done
        // If banana moved, find path and start BananaYumYum
        // If guard chasing, start OhNoGuard

        // If BananaYumYum:     // Done
        // Follow path
        // If reached end of path, start Idle
        // If guard chasing, start OhNoGuard

        // If OhNoGuard:

        //     If RunMode.RunBack:  // Done
        //     Follow path back to beginning of path
        //     If reached beginning, start Idle

        //     If RunMode.Scatter:  // Done
        //     Run away from guard
        //     If guard stops chasing, start Idle

        // If Captured:     // Done
        // For now Destroy own game object

        if (mood == MonkeMood.Captured)
        {
            gameobject.active = false;
            return;
        }
    }

    void FixedUpdate()
    {
        switch (mood)
        {
            case MonkeMood.BananaYumYum:
            {
                Vector3 move = Vector3.MoveTowards(rb.position, gridPath[currentPathIndex], settings.moveSpeed * Time.fixedDeltaTime);
                rb.MovePosition(move);

                if (((Vector3) rb.position - gridPath[currentPathIndex]).sqrMagnitude < 0.01f)
                {
                    currentPathIndex++;
                    if (currentPathIndex >= gridPath.Count)
                        mood = MonkeMood.Idle;
                }
            } break;

            case MonkeMood.OhNoGuard:
            {
                RunAway();
            } break;
        }
    }

    public void FindPathToBanana(PathFinder finder, Transform bananaTransform)
    {
        Vector3Int startPos = groundTilemap.WorldToCell(rb.position);

        Vector3Int bananaGridPosition = groundTilemap.WorldToCell(bananaTransform.position);
        Vector3Int endPos = FindPositionAroundBanana(bananaGridPosition);

        finder.UpdatePath(startPos, endPos, ref gridPath);

        currentPathIndex = 0;
        mood = MonkeMood.BananaYumYum;

        visual.SetTarget(bananaTransform, true);
    }

    public void StartRuningAway(Transform _guardTransform, MonkeRunMode mode)
    {
        if (mode == MonkeRunMode.RunBack)
            currentPathIndex--;

        guardTransform = _guardTransform;
        runMode = mode;
        mood = MonkeMood.OhNoGuard;

        visual.SetTarget(_guardTransform, false);
    }

    public void StopRuningAway()
    {
        mood = MonkeMood.Idle;
        visual.SetTarget(guardTransform, true);
    }

    void RunAway()
    {
        switch (runMode)
        {
            case MonkeRunMode.RunBack:
            {
                if (currentPathIndex < 0)
                {
                    StopRuningAway();
                    break;
                }

                Vector3 move = Vector3.MoveTowards(rb.position, gridPath[currentPathIndex], settings.moveSpeed * Time.fixedDeltaTime);
                rb.MovePosition(move);

                if (((Vector3) rb.position - gridPath[currentPathIndex]).sqrMagnitude < 0.01f)
                    currentPathIndex--;
            } break;

            case MonkeRunMode.Scatter:
            {
                Vector3 runDirection  = (transform.position - guardTransform.position).normalized;

                Vector3 firstProbe = transform.position + runDirection;

                {   // Try to move in direction opposite to guard
                    Vector3Int gridPosition = groundTilemap.WorldToCell(firstProbe);
                    if (groundTilemap.HasTile(gridPosition) && !levelTilemap.HasTile(gridPosition))
                    {
                        Vector3 dest = (Vector3) gridPosition + new Vector3(0.5f, 0.5f, 0f);
                        Vector3 move = Vector3.MoveTowards(rb.position, dest, settings.moveSpeed * Time.fixedDeltaTime);
                        rb.MovePosition(move);
                        break;
                    }
                }

                Vector3 perpendicular = Vector3.Cross(Vector3.forward, runDirection);

                {   // Try to move in direction of perpendicular
                    Vector3 probe = firstProbe + perpendicular;
                    Vector3Int gridPosition = groundTilemap.WorldToCell(probe);
                    if (groundTilemap.HasTile(gridPosition) && !levelTilemap.HasTile(gridPosition))
                    {
                        Vector3 dest = (Vector3) gridPosition + new Vector3(0.5f, 0.5f, 0f);
                        Vector3 move = Vector3.MoveTowards(rb.position, dest, settings.moveSpeed * Time.fixedDeltaTime);
                        rb.MovePosition(move);
                        break;
                    }
                }

                {   // Try to move in opposite direction of perpendicular
                    Vector3 probe = firstProbe - perpendicular;
                    Vector3Int gridPosition = groundTilemap.WorldToCell(probe);
                    if (groundTilemap.HasTile(gridPosition) && !levelTilemap.HasTile(gridPosition))
                    {
                        Vector3 dest = (Vector3) gridPosition + new Vector3(0.5f, 0.5f, 0f);
                        Vector3 move = Vector3.MoveTowards(rb.position, dest, settings.moveSpeed * Time.fixedDeltaTime);
                        rb.MovePosition(move);
                        break;
                    }
                }

                // Give Up!
                StopRuningAway();  // @Temp
            } break;
        }
    }

    Vector3Int FindPositionAroundBanana(Vector3Int bananaGridPosition)
    {
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
