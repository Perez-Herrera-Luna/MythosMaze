using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphNode
{
    private Vector2Int gridLocation;
    public int arenaRows;
    public int arenaCols;
    public float distToBoss;
    public int numDoors;

    // vectors representing door locations with respect to arenaCenter in gridScale
    public List<Vector2Int> activeDoorLocations;

    public Vector2Int GridLocation => gridLocation;

    public GraphNode(float bossDist, int doorNum)
    {
        distToBoss = bossDist;
        numDoors = doorNum;
    }

    public void SetInitialNodeValues(Vector2Int location, ArenaData arena, int gridScale)
    {
        gridLocation = location;
        arenaRows = arena.height / gridScale;
        arenaCols = arena.width / gridScale;
    }

    // helper function to convert arena location from vector2Int to Vector3 (for instantiation)
    public Vector3 ConvertArenaLocation(int gridScale)
    {
        return new Vector3(20 * gridScale * gridLocation.x , 0 , 20 * gridScale * gridLocation.y);
    }
}
