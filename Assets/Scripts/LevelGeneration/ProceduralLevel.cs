using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLevel : MonoBehaviour
{
    public LevelData currLevel;
    public int numArenas = 0;
    public GameObject bossArenaPrefab;
    public GameObject arenaPrefab;
    public GameObject questCharPrefab;

    public ArenaData bossArenaData;
    public List<ArenaData> arenasData;
    float maxCollisionRadius;

    private LevelGrid levelGrid;
    private LevelGraph levelGraph;

    // arena generation attributes
    Vector2Int currArenaLocation;
    List<Vector2Int> currArenaDistances;


    // walker algorithm attributes
    bool isInitialPath;
    int pathLength;
    List<Vector2Int> currPath;
    Vector2Int currWalkerLoc, targetLoc;


    // Start is called before the first frame update
    void Start()
    {
        // call necessary functions for procedural level generation

        InitialSetup();
        GenerateArenas();

    }

    // initialize grid and graph according to levelData
    private void InitialSetup()
    {
        levelGrid = new LevelGrid(currLevel.gridRows, currLevel.gridCols);
        levelGraph = new LevelGraph(currLevel.maxNumArenas);

        CalculateMaxCollisionRadius();
    }

    // helper function
    private void CalculateMaxCollisionRadius()
    {
        float maxCollisionRadius = 0;
        foreach(ArenaData arena in arenasData){
            int arenaRows = arenasData[0].height / levelGrid.GridScale;
            int arenaCols = arenasData[0].width / levelGrid.GridScale;

            Vector2Int arenaDimensions = new Vector2Int(arenaRows, arenaCols);
            arena.collisionRadius = arenaDimensions.magnitude;
            maxCollisionRadius = arena.collisionRadius > maxCollisionRadius ? arena.collisionRadius : maxCollisionRadius;
        }
    }

    // given available arenaData, calculate max dimension of all arenas

    // randomly generate arenas (< maxNumberArenas) in grid
    private void GenerateArenas()
    {
        // set boss arena: center of grid, 1 door
        AddArena(levelGrid.GridCenter, bossArenaData);

        // loop for each arena (until success or maxNumTries)
            // reset currArenaLocation
            // success = GenerateArenaLocation()		// randomly generates arena location, checks if is valid, retries limited # of times
            // if success, AddArena()
    }

    // after collecting arenaData and generating valid arena location, add arena to grid and graph datastructures
    private bool AddArena(Vector2Int location, ArenaData arena)
    {
        bool success = false;

        // add arena to grid
        success = levelGrid.AddArena(location, arena.height, arena.width);
        if(!success){
            Debug.Log("error adding arena to grid");
            return false;
        }

        // add arena to graph
        success = levelGraph.AddArena(location);
        if(!success){
            Debug.Log("error adding arena to grid");
            return false;
        }

        return success;
    }

    /*void LoadCombatArena()
    {
        GameObject arenaInstance;
        Arena arenaScript;
        arenaInstance = Instantiate(arenaPrefab, gameObject.transform);
        // need to add error checking if instantiated correctly

        arenaScript = arenaInstance.gameObject.GetComponent<Arena>();

        // set arena values (isEmpty, isBossLevel, hasCharacter, arenaLevel, numDoors)
        arenaScript.SetInitialValues(false, false, false, currLevel, 1);

        // if instantiated correctly add to list of arenas in level? 
    }*/
}
