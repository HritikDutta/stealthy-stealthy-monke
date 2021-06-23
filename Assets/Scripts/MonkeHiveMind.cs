using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class MonkeHiveMind : MonoBehaviour
{
    [Header("UI")]
    public Text monkeyCountText;

    [Header("Level")]
    public Tilemap groundTilemap;
    public Tilemap levelTilemap;

    [Header("Controls")]
    public Transform bananaTransform;

    [Header("Units")]
    public List<MonkeBehaviour> units = new List<MonkeBehaviour>();

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

        monkeyCountText.text = units.Count.ToString();
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
    }

    public void DemonStartedChasing(Transform guardTransform)
    {
        for (int i = 0; i < units.Count; i++)
            units[i].StartRuningAway(guardTransform);
    }

    public void DemonStoppedChasing()
    {
        for (int i = 0; i < units.Count; i++)
            units[i].StopRuningAway();
    }

    public void Captured(MonkeBehaviour monke)
    {
        // @Todo: Decide if monkey should be spared if it is able to hide just before getting caught
        if (monke.mood == MonkeMood.Hiding)
            return;

        monke.mood = MonkeMood.Captured;
        units.Remove(monke);
        monkeyCountText.text = units.Count.ToString();
    }
}
