using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum DemonState
{
    Patrolling,
    Investigating,
    Looking,
    Chasing,
    Capturing,
    Returning,
}

public class DemonBehaviour : MonoBehaviour
{
    [Header("Settings")]
    public DemonSettings settings;

    [Header("Movement")]
    public DemonState state;
    public Transform patrolPathTransform;

    [Header("Patrol")]
    public int sectionIndex;
    public Vector2Int viewAreaSize;
    public Vector2Int viewAreaPivot;    // @Todo: Have this as a per patrol waypoint kinda thing maybe? That might mean the demons have to be animated a bit...

    private MonkeSquad targetSquad;
    private MonkeBehaviour target;

    private Rigidbody2D rb;

    [HideInInspector]
    private List<Transform> patrolPath = new List<Transform>();
    private List<Vector3> gridPath = new List<Vector3>();
    private int patrolPathIndex;
    private int gridPathIndex;

    private int stamina;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        foreach (Transform waypoint in patrolPathTransform)
            patrolPath.Add(waypoint);

        Vector3Int currentGridPosition = Level.groundTilemap.WorldToCell(patrolPath[0].position);
        rb.position = (Vector3) currentGridPosition + new Vector3(0.5f, 0.5f, 0f);

        state = settings.startState;
        stamina = settings.maxStamina;

        patrolPathIndex = gridPathIndex = 0;
    }

    void Update()
    {
        if (Level.sectionsEnabled && Level.currentSectionIndex != sectionIndex)
            return;

        StartCoroutine(LightUpViewArea());

        switch (state)
        {
            case DemonState.Patrolling:
            case DemonState.Investigating:
            case DemonState.Looking:
            case DemonState.Returning:
            {
                if (stamina < settings.minStamina)
                    break;

                // @Temp: This should be a proper vision cone/rectangle thingy based on where the demon is looking

                Vector3Int currentGridPosition = Level.groundTilemap.WorldToCell(rb.position);
                Vector3Int offsetGridPosition = currentGridPosition + new Vector3Int(viewAreaPivot.x, viewAreaPivot.y, 0);
                Vector3Int areaTopLeft = offsetGridPosition + new Vector3Int(-viewAreaSize.x, viewAreaSize.y, 0);
                Vector3Int areaBottomRight = offsetGridPosition + new Vector3Int(viewAreaSize.x, -viewAreaSize.y, 0);
                
                foreach (MonkeSquad squad in Level.hiveMind.squads)
                {
                    if (state == DemonState.Chasing)
                        break;

                    foreach (MonkeBehaviour monke in squad.monkes)
                    {
                        if (monke.mood == MonkeMood.Hiding || monke.mood == MonkeMood.Captured)
                            continue;

                        Vector3Int monkeGridPosition = Level.groundTilemap.WorldToCell(monke.transform.position);
                        if (monkeGridPosition.x >= areaTopLeft.x && monkeGridPosition.x <= areaBottomRight.x &&
                            monkeGridPosition.y >= areaBottomRight.y && monkeGridPosition.y <= areaTopLeft.y)
                        {
                            state  = DemonState.Chasing;
                            targetSquad = squad;
                            target = monke;     // @Todo: Will it be more iteresting if a random monke was picked?

                            Level.hiveMind.DemonStartedChasing(squad, transform);

                            Level.finder.UpdatePath(currentGridPosition, monkeGridPosition, ref gridPath);
                            gridPathIndex = 0;

                            break;
                        }
                    }
                }
            } break;

            case DemonState.Capturing:
            {
                state = DemonState.Returning;

                Level.hiveMind.Captured(target);
                Level.hiveMind.DemonStoppedChasing(targetSquad);

                Vector3Int currentGridPosition = Level.groundTilemap.WorldToCell(rb.position);
                Vector3Int destGridPosition = Level.groundTilemap.WorldToCell(patrolPath[patrolPathIndex].position);

                Level.finder.UpdatePath(currentGridPosition, destGridPosition, ref gridPath);
                gridPathIndex = 0;
            } break;
        }
    }

    void FixedUpdate()
    {
        if (Level.sectionsEnabled && Level.currentSectionIndex != sectionIndex)
            return;

        switch (state)
        {
            case DemonState.Patrolling:
            {
                if (patrolPath.Count == 0)
                    break;

                Vector3 move = Vector3.MoveTowards(rb.position, patrolPath[patrolPathIndex].position, settings.patrolSpeed * Time.fixedDeltaTime);
                rb.MovePosition(move);

                if (((Vector3) rb.position - patrolPath[patrolPathIndex].position).sqrMagnitude < 0.01f)
                    patrolPathIndex = (patrolPathIndex + 1) % patrolPath.Count;
                
                if (stamina < settings.maxStamina)
                    stamina++;
            } break;

            case DemonState.Chasing:
            {
                Vector3Int currentGridPosition = Level.groundTilemap.WorldToCell(rb.position);

                if (stamina == 0 || target.mood == MonkeMood.Hiding || target.mood == MonkeMood.Captured || targetSquad.finished)
                {
                    state = DemonState.Returning;

                    Level.hiveMind.DemonStoppedChasing(targetSquad);

                    Vector3Int destGridPosition = Level.groundTilemap.WorldToCell(patrolPath[patrolPathIndex].position);
                    Level.finder.UpdatePath(currentGridPosition, destGridPosition, ref gridPath);
                    gridPathIndex = 0;

                    break;
                }

                Vector3Int currentGridPositionWithOffset = Level.groundTilemap.WorldToCell(rb.position + new Vector2(0.25f, 0.25f));
                Vector3Int monkeGridPosition = Level.groundTilemap.WorldToCell(target.transform.position);
                if (monkeGridPosition == currentGridPositionWithOffset)
                {
                    state = DemonState.Capturing;
                    break;
                }

                if (gridPathIndex >= gridPath.Count)
                {
                    Level.finder.UpdatePath(currentGridPosition, monkeGridPosition, ref gridPath);
                    gridPathIndex = 0;
                }

                Vector3 move = Vector3.MoveTowards(rb.position, gridPath[gridPathIndex], settings.chaseSpeed * Time.fixedDeltaTime);
                rb.MovePosition(move);

                if (((Vector3) rb.position - gridPath[gridPathIndex]).sqrMagnitude < 0.01f)
                {
                    gridPathIndex++;
                    stamina--;
                }
            } break;

            case DemonState.Investigating:
            {
                if (gridPathIndex >= gridPath.Count)
                {
                    state = DemonState.Looking;
                    StartCoroutine(LookForMonkey());
                    break;
                }

                Vector3 move = Vector3.MoveTowards(rb.position, gridPath[gridPathIndex], settings.patrolSpeed * Time.fixedDeltaTime);
                rb.MovePosition(move);

                if (((Vector3) rb.position - gridPath[gridPathIndex]).sqrMagnitude < 0.01f)
                    gridPathIndex++;
            } break;

            case DemonState.Returning:
            {
                if (gridPathIndex >= gridPath.Count)
                {
                    state = DemonState.Patrolling;
                    break;
                }

                Vector3 move = Vector3.MoveTowards(rb.position, gridPath[gridPathIndex], settings.patrolSpeed * Time.fixedDeltaTime);
                rb.MovePosition(move);

                if (((Vector3) rb.position - gridPath[gridPathIndex]).sqrMagnitude < 0.01f)
                {
                    gridPathIndex++;

                    if (stamina < settings.maxStamina)
                        stamina++;
                }
            } break;
        }
    }

    private IEnumerator LookForMonkey()
    {
        yield return new WaitForSeconds(settings.investigationTime);

        state = DemonState.Returning;

        Level.audio.Play("Demon Returning");

        Vector3Int currentGridPosition = Level.groundTilemap.WorldToCell(rb.position);
        Vector3Int destGridPosition = Level.groundTilemap.WorldToCell(patrolPath[patrolPathIndex].position);

        Level.finder.UpdatePath(currentGridPosition, destGridPosition, ref gridPath);
        gridPathIndex = 0;

        yield return null;
    }

    private IEnumerator LightUpViewArea()
    {
        Vector3Int currentGridPosition = Level.groundTilemap.WorldToCell(rb.position) + new Vector3Int(viewAreaPivot.x, viewAreaPivot.y, 0);
        for (int y = -viewAreaSize.y; y <= viewAreaSize.y; y++)
        {
            for (int x = -viewAreaSize.x; x <= viewAreaSize.x; x++)
            {
                Vector3Int gridPosition = currentGridPosition + new Vector3Int(x, y, 0);
                if (Level.groundTilemap.HasTile(gridPosition))
                {
                    Level.groundTilemap.SetTileFlags(gridPosition, TileFlags.None);
                    Level.groundTilemap.SetColor(gridPosition, Color.white);
                }
            }
        }

        yield return null;


        yield return new WaitForEndOfFrame();

        Color defaultTint = new Color(0.5170366f, 0.4730509f, 0.8301887f, 1f);

        // Could've cached this but meh...
        for (int y = -viewAreaSize.y; y <= viewAreaSize.y; y++)
        {
            for (int x = -viewAreaSize.x; x <= viewAreaSize.x; x++)
            {
                Vector3Int gridPosition = currentGridPosition + new Vector3Int(x, y, 0);
                if (Level.groundTilemap.HasTile(gridPosition))
                {
                    Level.groundTilemap.SetColor(gridPosition, defaultTint);
                    Level.groundTilemap.RefreshTile(gridPosition);  // Also resets tile flags back to TileFlags.LockColor
                }
            }
        }

        yield return null;
    }

    public void Investigate(Vector3Int positionToCheck)
    {
        if (state == DemonState.Investigating)
            return;
        
        Vector3Int currentGridPosition = Level.groundTilemap.WorldToCell(rb.position);

        Level.finder.UpdatePath(currentGridPosition, positionToCheck, ref gridPath);
        gridPathIndex = 0;
        state = DemonState.Investigating;
        
        Level.audio.Play("Demon Alert");

        Debug.Log("Investigating!");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        float x = 2f * (float) viewAreaSize.x + 1;
        float y = 2f * (float) viewAreaSize.y + 1;

        Vector3Int currentGridPosition = Level.groundTilemap.WorldToCell(transform.position) + new Vector3Int(viewAreaPivot.x, viewAreaPivot.y, 0);
        Vector3 displayPosition = (Vector3) currentGridPosition + new Vector3(0.5f, 0.5f, 0f);
        Gizmos.DrawWireCube(displayPosition, new Vector3(-x, y, 0.1f));
    }
}
