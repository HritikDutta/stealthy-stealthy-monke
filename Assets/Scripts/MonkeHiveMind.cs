using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MonkeHiveMind : MonoBehaviour
{
    [Header("Level")]
    public Tilemap groundTilemap;
    public Tilemap levelTilemap;

    [Header("Controls")]
    public Transform bananaTransform;
    public Transform guardTransform;
    public MonkeRunMode runMode;

    [Header("Units")]
    public List<MonkeMove>   units = new List<MonkeMove>();

    private Camera camera;
    private PathFinder finder;

    void Start()
    {
        camera = Camera.main;
        finder = GetComponent<PathFinder>();

        // @Temp: Bananas won't be visible from the start
        Vector3Int gridPosition = groundTilemap.WorldToCell(bananaTransform.position);
        Vector3 position = (Vector3) gridPosition;
        position.x += 0.5f;
        position.y += 0.5f;

        bananaTransform.position = position;

        for (int i = 0; i < units.Count; i++)
            units[i].FindPathToBanana(finder, bananaTransform);
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

            for (int i = 0; i < units.Count; i++)
                units[i].FindPathToBanana(finder, bananaTransform);
        }

        if (Input.GetMouseButtonDown(1))
        {
            for (int i = 0; i < units.Count; i++)
                units[i].StartRuningAway(guardTransform, runMode);
        }
    }
}
