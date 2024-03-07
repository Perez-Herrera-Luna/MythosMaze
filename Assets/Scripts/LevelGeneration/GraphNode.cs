using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphNode
{
    private Vector2Int gridLocation;
    public Vector2Int GridLocation => gridLocation;
    private int arenaRows;
    private int arenaCols;
    private float distToBoss;
    public float DistToBoss => distToBoss;
    private int maxNumDoors;
    public int MaxNumDoors { get => maxNumDoors; set => maxNumDoors = value; }

    // vectors representing door locations with respect to arenaCenter in gridScale
    private List<Vector2Int> availableDoors = new List<Vector2Int>();
    public int NumDoors => 4 - availableDoors.Count;

    public List<Vector2Int> GetAvailableDoors => availableDoors;

    public GraphNode(float bossDist, int doorNum)
    {
        distToBoss = bossDist;
        maxNumDoors = doorNum;
    }

    public void SetInitialNodeValues(Vector2Int location, ArenaData arena, int gridScale)
    {
        gridLocation = location;
        arenaRows = arena.height / gridScale;
        arenaCols = arena.width / gridScale;

        foreach(Vector2Int door in arena.doorLocations){
            availableDoors.Add(door);
        }
    }

    // helper function to convert arena location from vector2Int to Vector3 (for instantiation)
    public Vector3 ConvertArenaLocation(int gridScale)
    {
        return new Vector3(10 * gridScale * gridLocation.x , 0 , 10 * gridScale * gridLocation.y);
    }

    // helper function to generate Vector3 location for player start (called on source arena)
    public Vector3 PlayerInitLoc(int gridScale)
    {
        // for now set the player near the south door of the arena
        float offset = (10 * arenaRows * gridScale) / 2 - 20;

        Vector3 playerLoc = ConvertArenaLocation(gridScale);
        playerLoc.z -= offset;
        playerLoc.y += 1.4f;

        // Debug.Log(playerLoc);
        return playerLoc;
    }

    // helper function returns true if curr number of doors is less than maxNumDoors
    private bool CanAddDoor()
    {
        if(NumDoors < maxNumDoors){
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

            /* Debug.Log("Sorted Doors Arena: " + gridLocation.x + ", " + gridLocation.y);
            Debug.Log("Available Doors: " + availableDoors.Count);
            foreach(var doorL in sortedDoors.Values){
                Debug.Log("Door: " + doorL.x + ", " + doorL.y);
            } */

            return sortedDoors.Values.ToList();
        }else{
            return null;
        }
    }
}
