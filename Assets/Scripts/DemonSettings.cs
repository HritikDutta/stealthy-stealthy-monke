using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="NewDemonSettings", menuName="Settings/DemonSettings")]
public class DemonSettings : ScriptableObject
{
    public float patrolSpeed = 4f;
    public float chaseSpeed = 6f;

    public DemonState startState = DemonState.Patrolling;
    public int maxStamina = 10;
    public int minStamina = 4;
}
