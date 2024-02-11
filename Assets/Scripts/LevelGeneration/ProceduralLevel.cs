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
    // calls necessary functions for procedural level generation
    void Start()
    {
        InitialSetup();
        GenerateArenas();

        // run dijkstras on generated graph
        levelGraph.GenerateShortestPathTree();

        // GeneratePaths();

        LoadLevel();

    }

    // initialize grid and graph according to levelData
    private void InitialSetup()
    {
        levelGrid = new LevelGrid(currLevel.gridRows, currLevel.gridCols);
        levelGraph = new LevelGraph(currLevel);
    }

    // helper function
    // given available arenaData, calculate max dimension of all arenas
    private void CalculateMaxCollisionRadius()
    {
        float maxCollisionRadius = 0;
        foreach(ArenaData arena in arenasData){
            int arenaRows = arena.height / levelGrid.GridScale;
            int arenaCols = arena.width / levelGrid.GridScale;

            Vector2Int arenaDimensions = new Vector2Int(arenaRows, arenaCols);
            arena.collisionRadius = arenaDimensions.magnitude;
            maxCollisionRadius = arena.collisionRadius > maxCollisionRadius ? arena.collisionRadius : maxCollisionRadius;
        }
    }

    // randomly generate arenas (< maxNumberArenas) in grid
    private void GenerateArenas()
    {        
        // set boss arena: center of grid, 1 door
        levelGraph.AddArena(bossArenaData, levelGrid.GridCenter, levelGrid.GridScale);
        levelGrid.AddArena(levelGrid.GridCenter, bossArenaData.height, bossArenaData.width);
        numArenas++;

        Vector2Int arenaLocation;
        int numTriesArena = 0;
        int numTriesLoc = 0;
        bool validLoc = false, success = false;

        // loop for each arena (until success or maxNumTries)
        while((numArenas < currLevel.maxNumArenas) & (numTriesArena++ < currLevel.maxTriesGenArena))
        {
            numTriesLoc = 0;
            validLoc = false;
            do{
                // currently only using one type of arena prefab
                arenaLocation = levelGrid.GenerateArenaLocation(arenasData[0]);

                // try adding arena node to graph (checks if far enough distance from other arenas)
                validLoc = levelGraph.AddArena(arenasData[0], arenaLocation, levelGrid.GridScale);
            } while (!validLoc & (numTriesLoc++ < currLevel.maxTriesGenLoc));

            if(numTriesLoc > currLevel.maxTriesGenLoc){
                Debug.Log("Reached maxNumTries while trying to generate valid arena location");
            }

            // if managed to generate a valid location (within bounds, far enough from other arenas)
            if(validLoc){
                // try adding arena representation to grid
                success = levelGrid.AddArena(arenaLocation, arenasData[0].height, arenasData[0].width);
                if(!success){
                    Debug.Log("error adding arena to grid");
                    return;
                }

                numArenas++;
                numTriesArena = 0;
            }
        }

        if(numTriesArena > currLevel.maxTriesGenArena){
            Debug.Log("Reached maxNumTries while trying to generate new arena");
        }
    }

    // function which makes necessary calls to instantiate all arenas and paths in the level
    private void LoadLevel()
    {
        LoadArenas();

        // LoadPaths();
    }

    // function which loads all arenas in scene according to their locations in grid
    private void LoadArenas()
    {
        GameObject arenaInstance;
        Arena arenaScript;
        Vector3 arenaLocation;

        for(int i = 0; i < levelGraph.NumArenas; i++){
            arenaLocation = levelGraph.generatedArenas[i].ConvertArenaLocation(levelGrid.GridScale);
            arenaInstance = Instantiate(arenaPrefab, arenaLocation, Quaternion.identity, gameObject.transform);
            
            arenaScript = arenaInstance.gameObject.GetComponent<Arena>();
            // set arena initial values (isBossLevel, hasCharacter, arenaLevel, numDoors)
            arenaScript.SetInitialValues(false, false, currLevel, 1);
        }
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
