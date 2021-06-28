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
    public float hoverTime = 1f;

    [Header("Audio")]
    public string audioClipName;

    private Vector3Int gridPosition;
    private bool hasBeenCollected;

    private SpriteRenderer renderer;
    private Transform rendererTransform;

    void Awake()
    {
        if (type == CollectibleType.Letter)
            Level.AddLetter();
    }

    void Start()
    {
        gridPosition = Level.interactableTilemap.WorldToCell(transform.position);
        hasBeenCollected = false;

        rendererTransform = transform.GetChild(0);
        renderer = rendererTransform.GetComponent<SpriteRenderer>();
    }

    public void Collected(Transform collector)
    {
        if (hasBeenCollected)
            return;

        Level.audio.Play(audioClipName);

        StartCoroutine(PlayCollectAnimation(collector));
        hasBeenCollected = true;

        Level.interactableTilemap.SetColor(gridPosition, new Color(0f, 0f, 0f, 0f));

        if (type == CollectibleType.Letter)
            Level.LetterCollected();
    }

    private IEnumerator PlayCollectAnimation(Transform collector)
    {
        float startTime = Time.time;

        renderer.enabled = true;

        while (Time.time - startTime < hoverTime)
        {
            // Hover sprite over collector's head
            rendererTransform.position = collector.position + new Vector3(0f, 1.2f, 0f);
            yield return null;
        }

        renderer.enabled = false;
        
        Level.interactableTilemap.SetTile(gridPosition, null);
        Level.interactableTilemap.RefreshTile(gridPosition);
        yield return null;
    }
}
