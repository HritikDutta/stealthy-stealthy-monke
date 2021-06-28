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

    void Start()
    {
        gridPosition = Level.interactableTilemap.WorldToCell(transform.position);
        Level.interactableTilemap.SetTile(gridPosition, settings.intactSprite);

        broken = false;
    }

    public bool Break()
    {
        if (broken)
            return false;

        broken = true;

        gameObject.layer = 0;
        MakeSound();

        gridPosition = Level.interactableTilemap.WorldToCell(transform.position);
        Level.interactableTilemap.SetTile(gridPosition, settings.brokenSprite);
        Level.interactableTilemap.RefreshTile(gridPosition);

        return true;
    }

    private void MakeSound()
    {        
        Level.audio.Play(settings.audioClipName);

        GameObject ripple = Instantiate(settings.ripple, transform.position, Quaternion.identity);
        ParticleSystem ps = ripple.GetComponent<ParticleSystem>();
        ps.startSize = settings.soundRadius * 2.0f;
        ps.Play();

        DemonBehaviour closestDemon = null;
        float minSqrDistanceSoFar = float.MaxValue;

        Vector3 myPosition = (Vector3) gridPosition + new Vector3(0.5f, 0.5f, 0f);

        foreach (DemonBehaviour demon in Level.demons)
        {
            if (demon.state == DemonState.Investigating ||
                demon.state == DemonState.Chasing)
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

        gameObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, settings.soundRadius);
    }
}
