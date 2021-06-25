using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Shiny : MonoBehaviour
{
    [Header("Settings")]
    public ShinySettings settings;

    private bool broken = false;

    private Tilemap tilemap;
    private Vector3Int gridPosition;
    
    void Start()
    {
        tilemap = transform.parent.GetComponent<Tilemap>();
        gridPosition = tilemap.WorldToCell(transform.position);

        tilemap.SetTile(gridPosition, settings.intactSprite);
    }

    public void Break()
    {
        if (broken)
            return;

        tilemap.SetTile(gridPosition, settings.brokenSprite);
        tilemap.RefreshTile(gridPosition);

        gameObject.layer = 0;
        broken = true;

        MakeSound();
    }

    private void MakeSound()
    {        
        DemonBehaviour closestDemon = null;
        float minSqrDistanceSoFar = float.MaxValue;

        Vector3 myPosition = (Vector3) gridPosition + new Vector3(0.5f, 0.5f, 0f);

        foreach (DemonBehaviour demon in Level.demons)
        {
            if (demon.state == DemonState.Investigating)
                continue;
            
            float sqrDistance = (demon.transform.position - myPosition).sqrMagnitude;
            if (sqrDistance < minSqrDistanceSoFar)
            {
                minSqrDistanceSoFar = sqrDistance;
                closestDemon = demon;
            }
        }

        Debug.Log("Making Sound! " + closestDemon + " should investigate...");

        if (minSqrDistanceSoFar <= settings.soundRadius * settings.soundRadius)
            closestDemon.Investigate(gridPosition);
    }
}
