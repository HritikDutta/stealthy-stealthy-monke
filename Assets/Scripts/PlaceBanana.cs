using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// @Watch: https://youtu.be/YnwOoxtgZQI
// @Watch: https://learn.unity.com/tutorial/2d-roguelike-setup-and-assets?uv=5.x&projectId=5c514a00edbc2a0020694718#5c7f8528edbc2a002053b6f8

public class PlaceBanana : MonoBehaviour
{
    public Tilemap groundTilemap;
    public Tilemap levelTilemap;

    public Transform bananaTransform;

    private Camera camera;

    void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 screenPosition = Input.mousePosition;
            Vector3 worldPosition = camera.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0f;

            Vector3Int gridPosition = groundTilemap.WorldToCell(worldPosition);

            if (groundTilemap.HasTile(gridPosition) && !levelTilemap.HasTile(gridPosition))
            {
                Vector3 position = (Vector3) gridPosition;
                position.x += 0.5f;
                position.y += 0.5f;

                bananaTransform.position = position;
            }
        }
    }
}
