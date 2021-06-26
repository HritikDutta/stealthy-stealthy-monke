using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HidingSpot : MonoBehaviour
{
    public Tile openTile;
    public Tile closedTile;

    private Vector3Int gridPosition;
    public int numMonkeysHiding = 0;

    void Start()
    {
        gridPosition = Level.interactableTilemap.WorldToCell(transform.position);
        Level.interactableTilemap.SetTile(gridPosition, openTile);
    }

    public void HideMonkey()
    {
        if (numMonkeysHiding == 0)
        {
            Level.interactableTilemap.SetTile(gridPosition, closedTile);
            Level.interactableTilemap.RefreshTile(gridPosition);
        }

        numMonkeysHiding++;
    }

    public void UnhideMonkey()
    {
        numMonkeysHiding--;

        if (numMonkeysHiding == 0)
        {
            Level.interactableTilemap.SetTile(gridPosition, openTile);
            Level.interactableTilemap.RefreshTile(gridPosition);
        }
    }
}
