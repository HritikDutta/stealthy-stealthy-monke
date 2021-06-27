using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName="NewShinySettings", menuName="Settings/ShinySettings")]
public class ShinySettings : ScriptableObject
{
    [Header("Audio")]
    public string audioClipName;
    
    [Header("Gameplay")]
    public float soundRadius = 5.0f;

    [Header("Visual")]
    public Tile intactSprite;
    public Tile brokenSprite;
}
