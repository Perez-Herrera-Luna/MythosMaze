using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphNode
{
    private Vector2Int gridLocation;
    public Vector2Int GridLocation => gridLocation;
    private int arenaPrefabIndex;
    public int ArenaPrefabIndex => arenaPrefabIndex;
    private int arenaRows;
    private int arenaCols;
    private float distToBoss;
    public float DistToBoss => distToBoss;
    private int maxNumDoors = -1;
    private int initialDoorNum;
    public int MaxNumDoors => maxNumDoors;

    // vectors representing door locations with respect to arenaCenter and pathWidth
    private List<Vector2Int> availableDoors = new List<Vector2Int>();
    public int NumDoors => initialDoorNum - availableDoors.Count;

    public List<Vector2Int> GetAvailableDoors => availableDoors;

    public GraphNode(float bossDist, int maxDoors)
    {
        distToBoss = bossDist;
        maxNumDoors = maxDoors;
    }

    public void SetInitialNodeValues(Vector2Int location, ArenaData arena, int pathWidth, int prefabIndex)
    {
        gridLocation = location;
        arenaRows = arena.height / pathWidth;
        arenaCols = arena.width / pathWidth;
        arenaPrefabIndex = prefabIndex;

        maxNumDoors = (maxNumDoors == -1) ? arena.doorLocations.Count : maxNumDoors;

        foreach(Vector2Int door in arena.doorLocations){
            availableDoors.Add(door);
        }

        initialDoorNum = availableDoors.Count;

        // Debug.Log("adding arena at: " + gridLocation + " with maxNumDoors = " + maxNumDoors + " and initialDoorNum = " + initialDoorNum);
    }

    public void UpdateArenaDataValue(int arenaIndex, ArenaData newArena, int pathWidth)
    {
        arenaPrefabIndex = arenaIndex;

        arenaRows = newArena.height / pathWidth;
        arenaCols = newArena.width / pathWidth;

        maxNumDoors = (maxNumDoors == 1) ? maxNumDoors : newArena.doorLocations.Count;

        availableDoors.Clear();

        foreach (Vector2Int door in newArena.doorLocations)
        {
            availableDoors.Add(door);
        }

        initialDoorNum = availableDoors.Count;

        // Debug.Log("updating arena at: " + gridLocation + " with maxNumDoors = " + maxNumDoors + " and initialDoorNum = " + initialDoorNum);
    }

    // helper function to convert arena location from vector2Int to Vector3 (for instantiation)
    public Vector3 ConvertArenaLocation(int pathWidth)
    {
        float offsetX = 0.0f, offsetY = 0.0f;

        if ((arenaCols % 2) == 0)
            offsetX = -0.5f;

        if ((arenaRows % 2) == 0)
            offsetY = -0.5f;

        return new Vector3(10 * pathWidth * (gridLocation.x + offsetX), 0, 10 * pathWidth * (gridLocation.y + offsetY));
    }

    public int GetRotation(List<Vector2Int> possibleDoorLocs)
    {
        if(arenaPrefabIndex != -1)
        {
            Debug.Log("error: calling GetBossRotation on non-boss arena");
            return 0;
        }

        List<Vector2Int> currDoors = possibleDoorLocs.Except(availableDoors).ToList();

        if(currDoors.Count != 1)
        {
            Debug.Log("Error: GetBossRotation called on arena with more than one connected door");
            return 0;
        }

        int rotValue = 0;

        if (possibleDoorLocs.Count == 2)
        {
            if (currDoors[0] == possibleDoorLocs[1])
                rotValue = 180;
        }else if (possibleDoorLocs.Count == 4)
        {
            if (currDoors[0] == possibleDoorLocs[1])
                rotValue = 90;
            else if (currDoors[0] == possibleDoorLocs[2])
                rotValue = 180;
            else if (currDoors[0] == possibleDoorLocs[3])
                rotValue = 270;
        }
        else if (possibleDoorLocs.Count != 1)
        {
            Debug.Log("Error: invalid number of doors in boss arena data");
        }

        return rotValue;
    }

    // helper function to generate Vector3 location for player start (called on source arena)
    public Vector3 PlayerInitLoc(int pathWidth)
    {
        // for now set the player near the south door of the arena
        float offset = (10 * arenaRows * pathWidth) / 2 - 20;

        Vector3 playerLoc = ConvertArenaLocation(pathWidth);
        playerLoc.z -= offset;
        playerLoc.y += 1.4f;

        // Debug.Log("player initial position: " + playerLoc);
        return playerLoc;
    }

    // helper function returns true if curr number of doors is less than maxNumDoors
    private bool CanAddDoor()
    {
        if (NumDoors < maxNumDoors){
            return true;
        }else{
            Debug.Log("Error: trying to add more doors than maxNumDoors: " + maxNumDoors);
            return false;
        }
    }

    public bool AddDoor(Vector2Int doorDirection)
    {
        if(CanAddDoor()){
            return availableDoors.Remove(doorDirection);
        }else{
            return false;
        }
    }

    // returns the locations of curr arena available doors ordered based on proximity to target arena
    // if no available doors, returns null
    public List<Vector2Int> NearestDoors(GraphNode target)
    {
        if(CanAddDoor()){
            // calculate the direction of target
            Vector2 direction = target.GridLocation - gridLocation;

            SortedList<float, Vector2Int> sortedDoors = new SortedList<float, Vector2Int>();

            // sorts available doors by proximity to target arena direction
            foreach(Vector2Int door in availableDoors)
            {
                // calculate projection of door vector onto direction vector
                // dot product = negative (opposite directions), positive (same direction)
                float currDotProduct = Vector2.Dot(door, direction);

                // want to sort doors in order of closest (same direction) first, needs to have smallest value in queue
                float doorKey = (-1) * currDotProduct;
                
                // make dot product 'unique' if same as another door
                if(sortedDoors.ContainsKey(doorKey))
                    doorKey += 0.1f;
                
                sortedDoors.Add(doorKey, door);
            }

            return sortedDoors.Values.ToList();
        }else{
            return null;
        }
    }
}
