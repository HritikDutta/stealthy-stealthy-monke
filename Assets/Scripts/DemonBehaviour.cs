using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum DemonState
{
    Patrolling,
    Chasing,
    Capturing,
    Returning,
}

public class DemonBehaviour : MonoBehaviour
{
    [Header("Settings")]
    public DemonSettings settings;

    [Header("Level")]
    public Tilemap groundTilemap;
    public PathFinder finder;
    public MonkeHiveMind hiveMind;

    [Header("Movement")]
    public List<Vector3> patrolPath = new List<Vector3>();

    [Header("Behaviour")]
    public DemonState state;

    private Transform target;
    private int targetIndex;
    private Rigidbody2D rb;

    private List<Vector3> gridPath = new List<Vector3>();
    private int patrolPathIndex;
    private int gridPathIndex;

    private int stamina;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Vector3Int currentGridPosition = groundTilemap.WorldToCell(rb.position);
        rb.position = (Vector3) currentGridPosition + new Vector3(0.5f, 0.5f, 0f);

        state = settings.startState;
        stamina = settings.maxStamina;

        patrolPathIndex = gridPathIndex = 0;
    }

    void Update()
    {
        switch (state)
        {
            case DemonState.Patrolling:
            case DemonState.Returning:
            {
                if (stamina < settings.minStamina)
                    break;

                // @Temp: This should be a proper vision cone/rectangle thingy based on where the demon is looking

                Vector3Int currentGridPosition = groundTilemap.WorldToCell(rb.position);
                Vector3Int areaTopLeft = currentGridPosition + new Vector3Int(-1, 1, 0);
                
                for (int i = 0; i < hiveMind.units.Count; i++)
                {
                    Vector3Int monkeGridPosition = groundTilemap.WorldToCell(hiveMind.units[i].transform.position);
                    if (monkeGridPosition.x >= areaTopLeft.x && monkeGridPosition.x <= areaTopLeft.x + 2 &&
                        monkeGridPosition.y >= areaTopLeft.y - 2 && monkeGridPosition.y <= areaTopLeft.y)
                    {
                        target = hiveMind.units[i].transform;
                        targetIndex = i;
                        state  = DemonState.Chasing;

                        hiveMind.DemonStartedChasing(transform);

                        finder.UpdatePath(currentGridPosition, monkeGridPosition, ref gridPath);
                        gridPathIndex = 0;
                    }
                }
            } break;

            case DemonState.Capturing:
            {
                state = DemonState.Returning;

                hiveMind.Captured(hiveMind.units[targetIndex]);
                hiveMind.DemonStoppedChasing();

                Vector3Int currentGridPosition = groundTilemap.WorldToCell(rb.position);
                Vector3Int destGridPosition = groundTilemap.WorldToCell(patrolPath[patrolPathIndex]);

                finder.UpdatePath(currentGridPosition, destGridPosition, ref gridPath);
                gridPathIndex = 0;
                stamina = 0;
            } break;
        }
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case DemonState.Patrolling:
            {
                if (patrolPath.Count == 0)
                    break;

                Vector3 move = Vector3.MoveTowards(rb.position, patrolPath[patrolPathIndex], settings.patrolSpeed * Time.fixedDeltaTime);
                rb.MovePosition(move);

                if (((Vector3) rb.position - patrolPath[patrolPathIndex]).sqrMagnitude < 0.01f)
                    patrolPathIndex = (patrolPathIndex + 1) % patrolPath.Count;
            } break;

            case DemonState.Chasing:
            {
                Vector3Int currentGridPosition = groundTilemap.WorldToCell(rb.position);

                if (stamina == 0)
                {
                    state = DemonState.Returning;

                    hiveMind.DemonStoppedChasing();

                    Vector3Int destGridPosition = groundTilemap.WorldToCell(patrolPath[patrolPathIndex]);
                    finder.UpdatePath(currentGridPosition, destGridPosition, ref gridPath);
                    gridPathIndex = 0;

                    break;
                }

                Vector3Int currentGridPositionWithOffset = groundTilemap.WorldToCell(rb.position + new Vector2(0.25f, 0.25f));
                Vector3Int monkeGridPosition = groundTilemap.WorldToCell(target.position);
                if (monkeGridPosition == currentGridPositionWithOffset)
                {
                    state = DemonState.Capturing;
                    break;
                }

                if (gridPathIndex >= gridPath.Count)
                {
                    finder.UpdatePath(currentGridPosition, monkeGridPosition, ref gridPath);
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

            case DemonState.Returning:
            {
                if (gridPathIndex >= gridPath.Count)
                {
                    state = DemonState.Patrolling;
                    stamina = settings.maxStamina;
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
}
