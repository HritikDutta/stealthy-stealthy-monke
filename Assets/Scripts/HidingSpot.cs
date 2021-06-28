using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HidingSpot : MonoBehaviour
{
    public Sprite openTile;
    public Sprite closedTile;

    private Vector3Int gridPosition;
    public int numMonkeysHiding = 0;

    private SpriteRenderer renderer;

    void Start()
    {
        gridPosition = Level.interactableTilemap.WorldToCell(transform.position);
        renderer = GetComponent<SpriteRenderer>();
    }

    public void HideMonkey()
    {
        if (numMonkeysHiding == 0)
            renderer.sprite = closedTile;

        numMonkeysHiding++;
    }

    public void UnhideMonkey()
    {
        numMonkeysHiding--;

        if (numMonkeysHiding == 0)
            renderer.sprite = openTile;
    }
}
