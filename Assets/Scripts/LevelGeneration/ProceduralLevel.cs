using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLevel : MonoBehaviour
{
    public LevelData currLevel;
    public GameObject bossArenaPrefab;
    public List<GameObject> arenaPrefabs;
    public GameObject questCharPrefab;

    public GameObject pathPrefab;
    public GameObject turnPrefab;
    public GameObject threeJunctionPrefab;
    public GameObject fourJunctionPrefab;

    public ArenaData bossArenaData;
    public List<ArenaData> arenasData;
    private LevelGrid levelGrid;
    private LevelGraph levelGraph;

    // Start is called before the first frame update
    // calls necessary functions for procedural level generation
    void Start()
    {
        InitialSetup();
        GenerateArenas();

        // levelGrid.PrintGrid(); (debugging)

        GeneratePaths();

        LoadLevel();
    }

    // initialize grid and graph according to levelData
    private void InitialSetup()
    {
        levelGrid = new LevelGrid(currLevel);
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

        Vector2Int arenaLocation;
        int numTriesArena = 0;
        int numTriesLoc = 0;
        bool validLoc = false, success = false;

        // loop for each arena (until success or maxNumTries)
        while((levelGraph.NumArenasAdded < currLevel.maxNumArenas) & (numTriesArena++ < currLevel.maxTriesGenArena))
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
                
                numTriesArena = 0;
            }
        }

        if(numTriesArena > currLevel.maxTriesGenArena){
            Debug.Log("Reached maxNumTries while trying to generate new arena");
        }
    }

    // procedurally generate paths
    private void GeneratePaths()
    {
        // run dijkstras on generated graph
        levelGraph.GenerateShortestPathTree();

        // Debug.Log("GeneratePaths");

        // Initial Paths
        GraphNode srcArena, targetArena;

        // iterate through shortest path tree, generate initial paths
        foreach(KeyValuePair<int, int> kvp in levelGraph.ShortestPath){
            // try to generate path from prevNode to currNode in ShortestPath
            if(kvp.Key != levelGraph.SrcArenaIndex){
                // Debug.Log("generating initial path between src " + kvp.Value + " and dest " + kvp.Key);

                srcArena = levelGraph.GeneratedArenas[kvp.Value];
                targetArena = levelGraph.GeneratedArenas[kvp.Key];

                bool pathGenerated = levelGrid.GeneratePath(true, srcArena, targetArena);

                if(!pathGenerated)
                    Debug.Log("Error adding initial path");
                
            }
        }

        // Secondary Paths
        List<GraphNode> availableArenas = new List<GraphNode>();

        foreach(GraphNode arena in levelGraph.GeneratedArenas){
            if(arena.NumDoors < arena.MaxNumDoors)
                availableArenas.Add(arena);
        }

        while(availableArenas.Count > 0){
            // get first arena from availableArenas
            GraphNode currArena = availableArenas[0];
            availableArenas.RemoveAt(0);

            // if currArena was the last one in availableArenas
            if(availableArenas.Count == 0){
                if(currArena.NumDoors < 2){
                    Debug.Log("Error: Detached arena!");
                }
            }else{
                foreach(GraphNode availableArena in availableArenas){
                    // try and add a secondary path connection between currArena and availableArena
                    if(levelGrid.GeneratePath(false, currArena, availableArena)){
                        // check curr arena to see if can still add door
                        if(currArena.NumDoors < currArena.MaxNumDoors)
                            availableArenas.Add(currArena);

                        // check availableArena to see if need to remove
                        if(availableArena.NumDoors == availableArena.MaxNumDoors)
                            availableArenas.Remove(availableArena);

                        // break out of foreach loop
                        break;
                    }else{
                        Debug.Log("Error adding secondary path");
                    }
                }
            }
        }

    }

    // function which makes necessary calls to instantiate all arenas and paths in the level
    private void LoadLevel()
    {
        LoadArenas();

        // levelGrid.PrintGrid();
        LoadPaths();
    }

    // function which loads all arenas in scene according to their locations in grid
    private void LoadArenas()
    {
        GameObject arenaInstance;
        Arena arenaScript;
        Vector3 arenaLocation;

        for(int i = 0; i < levelGraph.NumArenasAdded; i++){
            arenaLocation = levelGraph.GeneratedArenas[i].ConvertArenaLocation(levelGrid.GridScale);
            arenaInstance = Instantiate(arenaPrefabs[0], arenaLocation, Quaternion.identity, gameObject.transform);
            
            arenaScript = arenaInstance.gameObject.GetComponent<Arena>();

            // set arena initial values (isBossLevel, hasCharacter, arenaLevel, numDoors)
            arenaScript.SetInitialValues(true, false, currLevel, levelGraph.GeneratedArenas[i].NumDoors);
        }

        if(levelGraph.NumArenasAdded != levelGraph.GeneratedArenas.Count){
            Debug.Log("Error in adding arenas");
        }
    }

    private void LoadPaths()
    {
        for (int i = 0; i < levelGrid.GridRows; i++)
        {
            for (int j = 0; j < levelGrid.GridCols; j++)
            {
                Vector2Int location = new Vector2Int(i, j);
                GridNode currNode = new GridNode();
                currNode = levelGrid.GetNode(location);
                
                // Debug.Log("checking currNode");

                Quaternion rotation;
                rotation = currNode.GetRotation();

                if (currNode.NodeValue == 'P')
                    GameObject.Instantiate(pathPrefab, levelGrid.ConvertLocation(i, j), rotation, gameObject.transform);
                else if (currNode.NodeValue == 'T')
                    GameObject.Instantiate(turnPrefab, levelGrid.ConvertLocation(i, j), rotation, gameObject.transform);

                if (currNode.NodeValue == 'J')
                {
                    if (currNode.NumConnections == 3)
                        GameObject.Instantiate(threeJunctionPrefab, levelGrid.ConvertLocation(i, j), rotation, gameObject.transform);
                    else
                        GameObject.Instantiate(fourJunctionPrefab, levelGrid.ConvertLocation(i, j), rotation, gameObject.transform);
                }
            }
        }

    }
}
