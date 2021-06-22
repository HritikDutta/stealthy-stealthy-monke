using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Shiny : MonoBehaviour
{
    public bool broken = false;

    private Tilemap tilemap;
    private Vector3Int gridPosition;
    
    void Start()
    {
        tilemap = transform.parent.GetComponent<Tilemap>();
        gridPosition = tilemap.WorldToCell(transform.position);
    }

    public void Break()
    {
        if (broken)
            return;

        tilemap.SetColor(gridPosition, Color.red);
        tilemap.RefreshTile(gridPosition);

        gameObject.SetActive(false);
        gameObject.layer = 0;
        broken = true;
    }
}
