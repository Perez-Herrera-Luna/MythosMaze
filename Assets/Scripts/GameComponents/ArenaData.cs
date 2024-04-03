using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu]
public class ArenaData : ScriptableObject
{
    // level procedural generation data
    public int height;
    public int width;

    public float collisionRadius;

    public bool isBossArena;

    // vectors representing door locations with respect to arenaCenter in world scale
    public List<Vector2Int> doorLocations;

    public List<string> enemyPrefabNames;
    public int maxNumEnemies;

    public List<string> powerupPrefabNames;

    public List<string> charPrefabNames;
}
