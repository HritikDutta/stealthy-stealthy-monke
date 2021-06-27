using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Shiny : MonoBehaviour
{
    [Header("Settings")]
    public ShinySettings settings;

    private bool broken = false;

    private Vector3Int gridPosition;

    private ParticleSystem breakRipple;
    
    void Start()
    {
        gridPosition = Level.interactableTilemap.WorldToCell(transform.position);
        Level.interactableTilemap.SetTile(gridPosition, settings.intactSprite);

        breakRipple = transform.GetChild(0).GetComponent<ParticleSystem>();
        breakRipple.startSize = settings.soundRadius * 2f;
    }

    public void Break()
    {
        if (broken)
            return;

        Level.interactableTilemap.SetTile(gridPosition, settings.brokenSprite);
        Level.interactableTilemap.RefreshTile(gridPosition);

        gameObject.layer = 0;
        broken = true;

        MakeSound();
    }

    private void MakeSound()
    {        
        breakRipple.Play();

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

        if (minSqrDistanceSoFar <= settings.soundRadius * settings.soundRadius)
        {
            Debug.Log("Making Sound! " + closestDemon + " should investigate...");
            closestDemon.Investigate(gridPosition);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, settings.soundRadius);
    }
}
