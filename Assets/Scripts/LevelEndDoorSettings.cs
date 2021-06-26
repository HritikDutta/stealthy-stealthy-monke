using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName="NewDoorSettings", menuName="Settings/DoorSettings")]
public class LevelEndDoorSettings : ScriptableObject
{
    public Tile closedTile;
    public Tile openTile;
}
