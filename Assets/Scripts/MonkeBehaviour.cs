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

public class MonkeBehaviour : MonoBehaviour
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

    private Rigidbody2D rb;

    private List<Vector3> gridPath;
    private int currentPathIndex;

    private Transform guardTransform;
    private MonkeRunMode runMode;

    // To find a spot on or around banana
    private static int[] gridYOffsets = new int[] { -1, -1, -1,   0, 0, 0,   1, 1, 1 };
    private static int[] gridXOffsets = new int[] { -1,  0,  1,  -1, 0, 1,  -1, 0, 1 };

    private static int[] neighboursX = new int[] { -1, 0,  0, 1 };
    private static int[] neighboursY = new int[] {  0, 1, -1, 0 };


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gridPath = new List<Vector3>();
        visual = transform.Find("visual").GetComponent<MonkeVisual>();
    }

    void LateUpdate()
    {
        if (mood == MonkeMood.Captured)
        {
            Destroy(gameObject);
            return;
        }
    }

    void FixedUpdate()
    {
        switch (mood)
        {
            case MonkeMood.BananaYumYum:
            {
                if (currentPathIndex >= gridPath.Count)
                {
                    mood = MonkeMood.Idle;
                    break;
                }

                Vector3 move = Vector3.MoveTowards(rb.position, gridPath[currentPathIndex], settings.moveSpeed * Time.fixedDeltaTime);
                rb.MovePosition(move);

                if (((Vector3) rb.position - gridPath[currentPathIndex]).sqrMagnitude < 0.01f)
                    currentPathIndex++;
            } break;

            case MonkeMood.OhNoGuard:
            {
                RunAway();
            } break;

            case MonkeMood.Idle:
            {
                Vector3Int gridPosition = groundTilemap.WorldToCell(rb.position);
                Vector3 dest = (Vector3) gridPosition + new Vector3(0.5f, 0.5f, 0f);

                if (((Vector3) rb.position - dest).sqrMagnitude >= 0.01f)
                {
                    Vector3 move = Vector3.MoveTowards(rb.position, dest, settings.moveSpeed * Time.fixedDeltaTime);
                    rb.MovePosition(move);
                }
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
        if (runMode == MonkeRunMode.RunBack)
            return;

        mood = MonkeMood.Idle;
        visual.SetTarget(guardTransform, true);
    }

    void RunAway()
    {
        switch (runMode)
        {
            case MonkeRunMode.RunBack:
            {
                if (currentPathIndex < 0)   // @Todo: What if the starting position was close by?? Maybe fall back to scatter? Or move-back more steps?? Soo many questions?????
                {
                    mood = MonkeMood.Idle;
                    visual.SetTarget(guardTransform, true);
                    break;
                }

                Vector3 move = Vector3.MoveTowards(rb.position, gridPath[currentPathIndex], settings.moveSpeed * Time.fixedDeltaTime);
                rb.MovePosition(move);

                if (((Vector3) rb.position - gridPath[currentPathIndex]).sqrMagnitude < 0.01f)
                    currentPathIndex--;
            } break;

            case MonkeRunMode.Scatter:
            {                
                // @Todo: Try to reduce chances of overlapping monkeys
                float[] moveOptions = new float[neighboursX.Length];
                Vector3Int gridPosition = groundTilemap.WorldToCell(rb.position);

                float maxDistSqr = (((Vector3) gridPosition + new Vector3(0.5f, 0.5f, 0f)) - guardTransform.position).sqrMagnitude;
                if (maxDistSqr > settings.minDistanceFromGuard)
                    break;

                Vector3 bestOptionPosition = transform.position;
                bool foundBetterOption = false;
                for (int i = 0; i < neighboursX.Length; i++)
                {
                    Vector3Int optionGridPosition = gridPosition + new Vector3Int(neighboursX[i], neighboursY[i], 0);
                    if (!groundTilemap.HasTile(optionGridPosition) || levelTilemap.HasTile(optionGridPosition))
                        continue;
                    
                    Vector3 dest = (Vector3) optionGridPosition + new Vector3(0.5f, 0.5f, 0f);
                    float distSqr = (dest - guardTransform.position).sqrMagnitude;

                    if ((maxDistSqr - distSqr) < -0.1f)
                    {
                        maxDistSqr = distSqr;
                        bestOptionPosition = dest;
                        foundBetterOption = true;
                    }
                }

                if (foundBetterOption)
                {
                    Vector3 move = Vector3.MoveTowards(rb.position, bestOptionPosition, settings.moveSpeed * Time.fixedDeltaTime);
                    rb.MovePosition(move);
                }
            } break;
        }
    }

    Vector3Int FindPositionAroundBanana(Vector3Int bananaGridPosition)
    {
        int offsetIndex = Random.Range(1, gridYOffsets.Length);

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
