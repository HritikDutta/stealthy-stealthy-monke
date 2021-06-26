using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum CollectibleType
{
    Key,
    Letter
}

public class Collectible : MonoBehaviour
{
    public CollectibleType type;
    public Sprite sprite;

    private Vector3Int gridPosition;

    void Start()
    {
        gridPosition = Level.interactableTilemap.WorldToCell(transform.position);
    }

    public void Collected()
    {
        if (type == CollectibleType.Key)
            Debug.Log("Collected Key!");
        else
            Debug.Log("Collected Letter!");

        Level.interactableTilemap.SetTile(gridPosition, null);
        Level.interactableTilemap.RefreshTile(gridPosition);
    }
}
