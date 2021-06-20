using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="NewMonkeSettings", menuName="Settings/MonkeSettings")]
public class MonkeSettings : ScriptableObject
{
    public float moveSpeed = 5f;

    [Header("Hopping")]
    public float hopSpeed  = 5f;
    public float gravity   = 20f;
    public float hopchance = 0.5f;
}
