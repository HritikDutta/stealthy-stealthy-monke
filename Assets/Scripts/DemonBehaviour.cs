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
    public MonkeHiveMind hiveMind;

    [Header("Movement")]
    public List<int> steps = new List<int>();

    [Header("Behaviour")]
    public DemonState state;

    private static int[] stepX = new int[] { -1,     0,   0,     1 };
    private static int[] stepY = new int[] {  0,     1,  -1,     0 };
    //                                        ^Left  ^Up  ^Down  ^Right

    private int currentStepIndex = 0;
    private Vector3Int currentGridPosition;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        currentGridPosition = groundTilemap.WorldToCell(rb.position);
        rb.position = (Vector3) currentGridPosition + new Vector3(0.5f, 0.5f, 0f);

        state = settings.startState;
    }

    void Update()
    {
        // If Patrolling:
        // Check surrounding tiles for monkeys
        // If monkey is found, save last grid cell and start Chasing

        // If Chasing:
        // Select the monkey and follow path to it.
        // If monkey grid position overlaps then start Capturing

        // If Capturing:
        // For now Destroy the monkey's game object
        // Start Returning

        // If Returning:
        // Check surrounding tiles for monkeys
        // If monkey is found, start Chasing
        // If reached last grid cell, start Patrolling
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case DemonState.Patrolling:
            {
                if (steps.Count == 0)
                    return;
                
                int moveIndex = steps[currentStepIndex];

                Vector3Int targetGridPosition = currentGridPosition + new Vector3Int(stepX[moveIndex], stepY[moveIndex], 0);
                Vector2 targetPosition = (Vector2) ((Vector3) targetGridPosition) + new Vector2(0.5f, 0.5f);
                
                Vector2 move = Vector2.MoveTowards(rb.position, targetPosition, settings.moveSpeed * Time.fixedDeltaTime);
                rb.MovePosition(move);

                if ((rb.position - targetPosition).sqrMagnitude < 0.01f)
                {
                    currentGridPosition = targetGridPosition;
                    currentStepIndex = (currentStepIndex + 1) % steps.Count;
                }
            } break;
        }
    }
}
