using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HidingSpot : MonoBehaviour
{
    public Tile openTile;
    public Tile closedTile;

    private Tilemap tilemap;
    private Vector3Int gridPosition;
    public int numMonkeysHiding = 0;

    void Start()
    {
        tilemap = transform.parent.GetComponent<Tilemap>();
        gridPosition = tilemap.WorldToCell(transform.position);

        tilemap.SetTile(gridPosition, openTile);
    }

    public void HideMonkey()
    {
        if (numMonkeysHiding == 0)
        {
            tilemap.SetTile(gridPosition, closedTile);
            tilemap.RefreshTile(gridPosition);
        }

        numMonkeysHiding++;
    }

    public void UnhideMonkey()
    {
        numMonkeysHiding--;

        if (numMonkeysHiding == 0)
        {
            tilemap.SetTile(gridPosition, openTile);
            tilemap.RefreshTile(gridPosition);
        }
    }
}
