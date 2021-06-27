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

    public bool Open(bool hasKey)
    {
        if (isOpen)
            return true;
        
        if (!hasKey)
        {
            // @Todo: Show prompt
            Debug.Log("Need key to open door!");
            return false;
        }

        Level.audio.Play("Door Open");

        Level.interactableTilemap.SetTile(gridPosition, settings.openTile);
        Level.interactableTilemap.RefreshTile(gridPosition);
        isOpen = true;

        return true;
    }
}
