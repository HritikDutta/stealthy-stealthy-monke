using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MouseOver : MonoBehaviour
{
    public Transform highlighter;

    private Camera camera;
    private bool mouseMoved = false;
    private SpriteRenderer highlightRenderer;

    private Vector3Int previousGridPosition;

    void Awake()
    {
        camera = GetComponent<Camera>();
        highlightRenderer = highlighter.GetComponent<SpriteRenderer>();
        previousGridPosition = new Vector3Int(0, 0, 10);
    }

    void Update()
    {
        Vector3 screenPosition = Input.mousePosition;
        Vector3 worldPosition = camera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;

        Vector3Int gridPosition = Level.groundTilemap.WorldToCell(worldPosition);
        if (gridPosition == previousGridPosition)
            return;

        if (Level.groundTilemap.HasTile(gridPosition) && !Level.wallTilemap.HasTile(gridPosition))
        {
            highlighter.position = (Vector3) gridPosition + new Vector3(0.5f, 0.5f, 0f);
            highlightRenderer.enabled = true;

            Debug.Log("Mouse Moved!");

            // @Todo: Different icons for interactables?
        }
        else
            highlightRenderer.enabled = false;
        
        previousGridPosition = gridPosition;
    }

    public void SetColor(Color color)
    {
        highlightRenderer.color = color;
    }
}
