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
    private bool redraw;

    private List<Vector3> gridPath = new List<Vector3>();

    private bool shouldHighlight = true;

    void Awake()
    {
        highlightRenderer = highlighter.GetComponent<SpriteRenderer>();

        camera = GetComponent<Camera>();
        previousGridPosition = new Vector3Int(0, 0, 10);
    }

    void Update()
    {
        if (!shouldHighlight)
            return;

        Vector3 screenPosition = Input.mousePosition;
        Vector3 worldPosition = camera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;

        Vector3Int gridPosition = Level.groundTilemap.WorldToCell(worldPosition);
        if (gridPosition == previousGridPosition && !redraw)
            return;

        if (IsInSection(gridPosition) && Level.groundTilemap.HasTile(gridPosition) && !Level.wallTilemap.HasTile(gridPosition))
        {
            highlighter.position = (Vector3) gridPosition + new Vector3(0.5f, 0.5f, 0f);
            highlightRenderer.enabled = true;

            // @Todo: Draw this path onto the screen with a dotted line - Taking too much time
            // Will need to do this manually since Unity's line renderer SUCKS ASS UGHHHHHH!!

            // @Todo: Different icons for interactables?
        }
        else
            highlightRenderer.enabled = false;
        
        previousGridPosition = gridPosition;
        redraw = false;
    }

    private bool IsInSection(Vector3Int position)
    {
        return position.y < Level.sectionTop && position.y >= Level.sectionBottom;
    }

    public void Disable()
    {
        highlightRenderer.enabled = false;
        shouldHighlight = false;
    }

    public void Enable()
    {
        highlightRenderer.enabled = true;
        shouldHighlight = true;
    }

    public void SetColor(Color _color)
    {
        highlightRenderer.color = _color;
        shouldHighlight = true;
        redraw = true;
    }
}
