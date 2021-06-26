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

    void Start()
    {
        camera = GetComponent<Camera>();
        highlightRenderer = highlighter.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        mouseMoved = (Input.GetAxis("Mouse X") != 0.1f || Input.GetAxis("Mouse Y") != 0.1f);
    }

    void LateUpdate()
    {
        if (!mouseMoved)
            return;

        Vector3 screenPosition = Input.mousePosition;
        Vector3 worldPosition = camera.ScreenToWorldPoint(screenPosition);
        worldPosition.z = 0f;

        Vector3Int gridPosition = Level.groundTilemap.WorldToCell(worldPosition);

        if (Level.groundTilemap.HasTile(gridPosition) && !Level.wallTilemap.HasTile(gridPosition))
        {
            highlighter.position = (Vector3) gridPosition + new Vector3(0.5f, 0.5f, 0f);
            highlightRenderer.enabled = true;

            // @Todo: Different icons for interactables?
        }
        else
        {
            highlightRenderer.enabled = false;
        }
    }

    public void SetColor(Color color)
    {
        highlightRenderer.color = color;
    }
}
