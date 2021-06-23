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
    Hiding,
}

public class MonkeBehaviour : MonoBehaviour
{
    [Header("Behaviour")]
    public MonkeMood mood;

    [Header("Settings")]
    public MonkeSettings settings;

    [Header("Level")]
    public Tilemap itemTilemap;
    public Tilemap groundTilemap;
    public Tilemap levelTilemap;

    [Header("Visual")]
    public MonkeVisual visual;

    private Rigidbody2D rb;

    private List<Vector3> gridPath;
    private int currentPathIndex;

    private Transform guardTransform;

    // To find a spot on or around banana
    private static int[] gridYOffsets = new int[] { -1, -1, -1,   0, 0, 0,   1, 1, 1 };
    private static int[] gridXOffsets = new int[] { -1,  0,  1,  -1, 0, 1,  -1, 0, 1 };

    private static int[] neighboursX = new int[] { -1, 0,  0, 1 };
    private static int[] neighboursY = new int[] {  0, 1, -1, 0 };

    private bool checkedSurroundings = false;
    private Vector3 restPosition;
    private bool lookForHidingSpot = true;

    private HidingSpot currentHidingSpot;
    private bool wasHiding = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gridPath = new List<Vector3>();
        visual = transform.Find("visual").GetComponent<MonkeVisual>();
        restPosition = (Vector3) groundTilemap.WorldToCell(rb.position) + new Vector3(0.5f, 0.5f, 0f);
    }

    void LateUpdate()
    {
        if (mood == MonkeMood.Captured)
        {
            Destroy(gameObject);
            return;
        }

        // @Todo: Decide if you want the banana to be placed on the hiding spot
        // @Todo: Look at the number of monkeys in the spot and select best option for better distribution
        switch (mood)
        {
            case MonkeMood.OhNoGuard:
            {
                Vector3Int gridPosition = groundTilemap.WorldToCell(rb.position);
                for (int i = 0; i < gridXOffsets.Length; i++)
                {
                    Vector3Int probePosition = gridPosition + new Vector3Int(gridXOffsets[i], gridYOffsets[i], 0);
                    if (itemTilemap.HasTile(probePosition))
                    {
                        GameObject item = itemTilemap.GetInstantiatedObject(probePosition);

                        int layer = 1 << item.layer;
                        if (layer == LayerMask.GetMask("HidingSpot"))
                        {
                            currentHidingSpot = item.GetComponent<HidingSpot>();
                            currentHidingSpot.HideMonkey();
                            visual.Hide();
                            mood = MonkeMood.Hiding;
                            break;
                        }
                    }
                }
            } break;

            case MonkeMood.BananaYumYum:
            {
                Vector3Int gridPosition = groundTilemap.WorldToCell(rb.position);
                for (int i = 0; i < gridXOffsets.Length; i++)
                {
                    Vector3Int probePosition = gridPosition + new Vector3Int(gridXOffsets[i], gridYOffsets[i], 0);
                    if (itemTilemap.HasTile(probePosition))
                    {
                        GameObject item = itemTilemap.GetInstantiatedObject(probePosition);
                        int layer = 1 << item.layer;

                        // @Todo: Decide if monkeys should break stuff on the way or not                        
                        if (layer == LayerMask.GetMask("Shiny"))
                        {
                            Shiny shiny = item.GetComponent<Shiny>();
                            gridPath[currentPathIndex] = (Vector3) probePosition + new Vector3(0.5f, 0.5f, 0f);
                            shiny.Break();
                            break;
                        }
                    }
                }
            } break;

            case MonkeMood.Idle:
            {
                if (checkedSurroundings)
                    break;

                Vector3Int gridPosition = groundTilemap.WorldToCell(rb.position);
                for (int i = 0; i < gridXOffsets.Length; i++)
                {
                    Vector3Int probePosition = gridPosition + new Vector3Int(gridXOffsets[i], gridYOffsets[i], 0);
                    if (itemTilemap.HasTile(probePosition))
                    {
                        GameObject item = itemTilemap.GetInstantiatedObject(probePosition);
                        int layer = 1 << item.layer;

                        if (layer == LayerMask.GetMask("Shiny"))
                        {
                            Shiny shiny = item.GetComponent<Shiny>();
                            restPosition = (Vector3) probePosition + new Vector3(0.5f, 0.5f, 0f);
                            shiny.Break();
                            break;
                        }

                        if (lookForHidingSpot && layer == LayerMask.GetMask("HidingSpot"))
                        {
                            currentHidingSpot = item.GetComponent<HidingSpot>();
                            currentHidingSpot.HideMonkey();
                            visual.Hide();
                            mood = MonkeMood.Hiding;
                            wasHiding = true;
                            break;
                        }
                    }
                }

                checkedSurroundings = true;
            } break;
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
                    checkedSurroundings = false;
                    lookForHidingSpot = true;
                    restPosition = gridPath[currentPathIndex - 1];
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
                if (((Vector3) rb.position - restPosition).sqrMagnitude >= 0.01f)
                {
                    Vector3 move = Vector3.MoveTowards(rb.position, restPosition, settings.moveSpeed * Time.fixedDeltaTime);
                    rb.MovePosition(move);
                }
            } break;
        }
    }

    public void FindPathToBanana(PathFinder finder, Transform bananaTransform)
    {
        // Ignore banana if being chased
        if (mood == MonkeMood.OhNoGuard)
            return;

        if (mood == MonkeMood.Hiding)
        {
            currentHidingSpot.UnhideMonkey();
            visual.Unhide();
            lookForHidingSpot = false;
        }

        Vector3Int startPos = groundTilemap.WorldToCell(rb.position);

        Vector3Int bananaGridPosition = groundTilemap.WorldToCell(bananaTransform.position);
        Vector3Int endPos = FindPositionAroundBanana(bananaGridPosition);

        finder.UpdatePath(startPos, endPos, ref gridPath);

        currentPathIndex = 0;
        mood = MonkeMood.BananaYumYum;

        visual.SetTarget(bananaTransform, true);
    }

    public void StartRuningAway(Transform _guardTransform)
    {
        guardTransform = _guardTransform;
        mood = MonkeMood.OhNoGuard;
        lookForHidingSpot = false;

        visual.SetTarget(_guardTransform, false);
    }

    public void StopRuningAway()
    {
        if (mood == MonkeMood.Hiding)
        {
            if (wasHiding)
                return;
            
            currentHidingSpot.UnhideMonkey();
            visual.Unhide();
            lookForHidingSpot = false;
        }

        mood = MonkeMood.Idle;
        restPosition = (Vector3) groundTilemap.WorldToCell(rb.position) + new Vector3(0.5f, 0.5f, 0f);
        visual.SetTarget(guardTransform, true);
    }

    void RunAway()
    {
        // @Todo: Try to reduce chances of overlapping monkeys
        float[] moveOptions = new float[neighboursX.Length];
        Vector3Int gridPosition = groundTilemap.WorldToCell(rb.position);

        float maxDistSqr = (((Vector3) gridPosition + new Vector3(0.5f, 0.5f, 0f)) - guardTransform.position).sqrMagnitude;
        if (maxDistSqr > settings.minDistanceFromGuard)
            return;

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
