using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphNode
{
    public Vector2Int gridLocation;
    public int arenaRows;
    public int arenaCols;
    public int distToBoss;
    public int numDoors;

    // vectors representing door locations with respect to arenaCenter in gridScale
    public List<Vector2Int> activeDoorLocations;

    public void SetInitialNodeValues(Vector2Int location, ArenaData arena, int gridScale)
    {
        gridLocation = location;
        arenaRows = arena.height / gridScale;
        arenaCols = arena.width / gridScale;
    }
}
