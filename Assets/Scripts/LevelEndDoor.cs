using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndDoor : MonoBehaviour
{
    public LevelEndDoorSettings settings;
    
    private Vector3Int gridPosition;
    private bool isOpen;

    void Start()
    {
        gridPosition = Level.interactableTilemap.WorldToCell(transform.position);
        Level.interactableTilemap.SetTile(gridPosition, settings.closedTile);
        Level.interactableTilemap.RefreshTile(gridPosition);
        isOpen = false;

        Level.AddDoorTile(this);
    }

    public void Open(bool hasKey)
    {
        if (isOpen)
            return;
        
        if (!hasKey)
        {
            Debug.Log("Need key to open door!");
            return;
        }

        Level.interactableTilemap.SetTile(gridPosition, settings.openTile);
        Level.interactableTilemap.RefreshTile(gridPosition);
        isOpen = true;

        Debug.Log("End of level!");
    }
}
