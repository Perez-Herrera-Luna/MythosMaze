using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class ArenaData : ScriptableObject
{
    // level procedural generation data
    public int height;
    public int width;

    public float collisionRadius;
    // vectors representing door locations with respect to arenaCenter in world scale
    public List<Vector2Int> doorLocations;

    // arena procedural generation data
    public int maxEnemies;
    public float maxEnemyPower;
    public int maxPowerups; 
}
