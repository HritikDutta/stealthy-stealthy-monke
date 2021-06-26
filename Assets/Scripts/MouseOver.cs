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
    private Vector3Int startPosition;
    private bool redraw;

    private List<Vector3> gridPath = new List<Vector3>();

    void Awake()
    {
        highlightRenderer = highlighter.GetComponent<SpriteRenderer>();

        camera = GetComponent<Camera>();
        previousGridPosition = new Vector3Int(0, 0, 10);
        startPosition = new Vector3Int(0, 0, 10);
    }

    void Update()
    {
        Vector3 screenPosition = Input.mousePosition;
        Vector3 worldPosition = camera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;

        Vector3Int gridPosition = Level.groundTilemap.WorldToCell(worldPosition);
        if (gridPosition == previousGridPosition && !redraw)
            return;

        if (Level.groundTilemap.HasTile(gridPosition) && !Level.wallTilemap.HasTile(gridPosition))
        {
            highlighter.position = (Vector3) gridPosition + new Vector3(0.5f, 0.5f, 0f);
            highlightRenderer.enabled = true;

            Level.finder.UpdatePath(startPosition, gridPosition, ref gridPath);

            // @Todo: Draw this path onto the screen with a dotted line
            // Will need to do this manually since Unity's line renderer SUCKS ASSS UGHHHHHH!!

            // @Todo: Different icons for interactables?
        }
        else
            highlightRenderer.enabled = false;
        
        previousGridPosition = gridPosition;
        redraw = false;
    }

    public void SetColorAndStart(Color _color, Vector3 _startPosition)
    {
        highlightRenderer.color = _color;
        startPosition = Level.groundTilemap.WorldToCell(_startPosition);
        redraw = true;
    }
}
