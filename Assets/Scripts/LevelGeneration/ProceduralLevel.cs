using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class ProceduralLevel : MonoBehaviour
{
    public LevelData currLevel;

    public ArenaData bossArenaData;
    public GameObject bossArenaPrefab;

    public ArenaData srcArenaData;
    public GameObject srcArenaPrefab;

    public List<ArenaData> arenasData;
    public List<GameObject> arenaPrefabs;

    public GameObject questCharPrefab;

    public GameObject pathPrefab;
    public GameObject turnPrefab;
    public GameObject threeJunctionPrefab;
    public GameObject fourJunctionPrefab;

    public LevelGraph GetLevelGraph => levelGraph;  // for unit/integration testing 

    private LevelGrid levelGrid;
    private LevelGraph levelGraph;

    private GameObject playerObj;

    private int maxTriesGenLoc = 500;
    private int maxTriesGenArena = 20;
    private int maxTriesLevelGen = 3;

    // Start is called before the first frame update
    // calls necessary functions for procedural level generation
    void Start()
    {
        InitialSetup();
    }

    // asynchronous function to load the player level
    public async Task GenerateLevelAsync()
    {
        await Task.Run(() => GenerateLevel());
    }

    private void GenerateLevel()
    {
        bool noDeadEnds = true;
        int numTries = 1;

        do
        {
            if (!noDeadEnds)
            {
                levelGrid.ResetGrid();
                levelGraph.ResetGraph();
            }

            GenerateArenas();

            noDeadEnds = GeneratePaths();
        } while (!noDeadEnds & (numTries++ < maxTriesLevelGen));

        if (numTries == (maxTriesLevelGen + 1))
            Debug.Log("error: reached maxTriesLevelGen => generated level has at least one dead end");
       
    }

    // initialize grid and graph according to levelData
    private void InitialSetup()
    {
        levelGrid = new LevelGrid(currLevel);
        levelGraph = new LevelGraph(currLevel, srcArenaData);
    }

    private ArenaData GenerateRandomArenaPrefab()
    {
        int randNum = ThreadSafeRandom.GetRandom(0, arenasData.Count);

        return arenasData[randNum];
    }

    // randomly generate arenas (< maxNumberArenas) in grid
    private void GenerateArenas()
    {
        bool generateArenasSuccess = true;
        int maxTries = 10;
        int numTries = 0;

        do
        {
            generateArenasSuccess = true;
            int currArenaIndex = -1;

            // set boss arena: center of grid, 1 door
            levelGraph.AddArena(bossArenaData, 0, levelGrid.GridCenter, levelGrid.PathWidth, levelGrid.GridRadius);
            levelGrid.AddArena(levelGrid.GridCenter, currArenaIndex, bossArenaData);

            Vector2Int arenaLocation;
            int numTriesArena = 0;
            int numTriesLoc = 0;
            bool validLoc = false, locSuccess = false;
            int randomArenaIndex = 0;

            // loop for each arena (until success or maxNumTries)
            while((levelGraph.NumArenasAdded < currLevel.maxNumArenas) & (numTriesArena++ < maxTriesGenArena))
            {
                numTriesLoc = 0;
                validLoc = false;
                do{
                    if (levelGraph.NumArenasAdded < arenasData.Count + 1)
                        randomArenaIndex = levelGraph.NumArenasAdded - 1;
                    else
                        randomArenaIndex = ThreadSafeRandom.GetRandom(0, arenasData.Count);

                    arenaLocation = levelGrid.GenerateArenaLocation(arenasData[randomArenaIndex]);

                    // try adding arena node to graph (checks if far enough distance from other arenas)
                    validLoc = levelGraph.AddArena(arenasData[randomArenaIndex], randomArenaIndex, arenaLocation, levelGrid.PathWidth, levelGrid.GridRadius);
                } while (!validLoc & (numTriesLoc++ < maxTriesGenLoc));

                if(numTriesLoc > maxTriesGenLoc){
                    Debug.Log("Reached maxNumTries while trying to generate valid arena location");
                    validLoc = false;
                }

                // if managed to generate a valid location (within bounds, far enough from other arenas)
                if(validLoc){
                    currArenaIndex++;
                    // try adding arena representation to grid
                    locSuccess = levelGrid.AddArena(arenaLocation, currArenaIndex, arenasData[randomArenaIndex]);
                    if(!locSuccess){
                        currArenaIndex--;
                        Debug.Log("error adding arena to grid");
                        generateArenasSuccess = false;
                    }
                
                    numTriesArena = 0;
                }
            }

            if(numTriesArena > maxTriesGenArena){
                Debug.Log("Reached maxNumTries while trying to generate new arena");
                generateArenasSuccess = false;
            }

            // calculate source arena index of generated level graph
            if (generateArenasSuccess)
                generateArenasSuccess = levelGraph.CalculateSourceArenaIndex(levelGrid.PathWidth);

            int srcIndex = levelGraph.SrcArenaIndex;
            int previousPrefabIndex = levelGraph.GeneratedArenas[srcIndex].ArenaPrefabIndex;

            if (generateArenasSuccess)
                generateArenasSuccess = levelGrid.UpdateArena(levelGraph.GeneratedArenas[srcIndex].GridLocation, srcIndex, srcArenaData, arenasData[previousPrefabIndex]);

        } while (!generateArenasSuccess & (numTries++ < maxTries));

        if(numTries == maxTries)
            Debug.Log("error: reached maxTries in generating arenas");
    }

    // procedurally generate paths
    private bool GeneratePaths()
    {
        // run dijkstras on generated graph
        levelGraph.GenerateShortestPathTree();
        
        // Initial Paths
        GraphNode srcArena, targetArena;

        // iterate through shortest path tree, generate initial paths
        foreach(KeyValuePair<int, int> kvp in levelGraph.ShortestPath){

            // try to generate path from prevNode to currNode in ShortestPath
            if(kvp.Key != levelGraph.SrcArenaIndex){
                srcArena = levelGraph.GeneratedArenas[kvp.Value];
                targetArena = levelGraph.GeneratedArenas[kvp.Key];

                bool pathGenerated = levelGrid.GeneratePath(true, srcArena, kvp.Value, targetArena, kvp.Key);                
            }
        }

        // Secondary Paths

        SortedList<float, (int, GraphNode)> availableArenas = new SortedList<float, (int, GraphNode)>();

        for(int i = 0; i < levelGraph.GeneratedArenas.Count; i++)
        {
            GraphNode arena = levelGraph.GeneratedArenas[i];
            (int, GraphNode) availableArena = (i, arena);

            if (arena.NumDoors < arena.MaxNumDoors)
            {
                float numDoors = arena.NumDoors;

                // ensure sortedList key is unique
                if (availableArenas.ContainsKey(numDoors))
                    numDoors -= 0.1f * i;

                availableArenas.Add(numDoors, availableArena);
            }
        }

        while (availableArenas.Count > 0){
            (int srcIndex, GraphNode srcArena) source = availableArenas.Values[0];
            availableArenas.RemoveAt(0);

            if (source.srcIndex >= levelGraph.GeneratedArenas.Count)
            {
                Debug.Log("Error: Incorect values for arena indexes");
                return false;
            }
            else if (availableArenas.Count == 0)
            {
                if (source.srcArena.NumDoors < 2)
                {
                    Debug.Log("Error: Arena with only one door");
                    return false;
                }
                else
                {
                    return true;
                }
            }

            bool addDoorSuccess = false;
            (int index, GraphNode arena) endArena = (source.srcIndex, source.srcArena);

            foreach((int index, GraphNode arena) in availableArenas.Values){
                // try and add a secondary path connection between currArena and availableArena
                if (levelGrid.GeneratePath(false, source.srcArena, source.srcIndex, arena, index))
                {
                    // if successful path, break out of foreach loop
                    addDoorSuccess = true;
                    endArena = (index, arena);
                    break;
                }
            }

            if (addDoorSuccess)
            {
                // check updated curr arena to see if can still add door(s)
                if (source.srcArena.NumDoors < source.srcArena.MaxNumDoors)
                {
                    float numDoors = source.srcArena.NumDoors;

                    // ensure sortedList key is unique
                    while (availableArenas.ContainsKey(numDoors))
                    {
                        numDoors -= 0.1f;
                    }

                    availableArenas.Add(numDoors, (source.srcIndex, source.srcArena));
                }

                // check endArena to see if need to remove from availableArenas
                if (endArena.arena.NumDoors == endArena.arena.MaxNumDoors)
                {
                    int removeIndex = availableArenas.IndexOfValue(endArena);

                    if (removeIndex == -1)
                        Debug.Log("Error: cannot find targetArena in sortedList of availableArenas");
                    else
                        availableArenas.RemoveAt(removeIndex);
                }
            } else if(availableArenas.Count == 1)
            {
                (int index, GraphNode arena) remaining = availableArenas.Values[0];
                availableArenas.RemoveAt(0);

                // try adding path between final arenas in opposite direction
                if(!levelGrid.GeneratePath(false, remaining.arena, remaining.index, source.srcArena, source.srcIndex))
                {
                    Debug.Log("Error: Arena(s) with only one door");
                    return false;
                }
                else
                {
                    if (remaining.arena.NumDoors < remaining.arena.MaxNumDoors)
                    {
                        float numDoors = remaining.arena.NumDoors;

                        // ensure sortedList key is unique
                        while (availableArenas.ContainsKey(numDoors))
                        {
                            numDoors -= 0.1f;
                        }

                        availableArenas.Add(numDoors, remaining);
                    }
                }
            }
        }

        return true;
    }

    // function which makes necessary calls to instantiate all arenas and paths in the level
    public void LoadLevel()
    {
        // levelGrid.PrintGrid();
        
        LoadArenas();

        LoadPaths();
    }

    // function which loads all arenas in scene according to their locations in grid
    private void LoadArenas()
    {
        GameObject arenaInstance;
        Arena arenaScript;
        Vector3 arenaLocation;

        for(int i = 0; i < levelGraph.NumArenasAdded; i++){
            arenaLocation = levelGraph.GeneratedArenas[i].ConvertArenaLocation(levelGrid.PathWidth);

            GameObject arenaPrefab;
            Quaternion rotation = Quaternion.identity;
            bool sourceArena = false;

            if (i == 0) {
                arenaPrefab = bossArenaPrefab;
                rotation.eulerAngles = new Vector3(0, levelGraph.GeneratedArenas[i].GetRotation(bossArenaData.doorLocations), 0);
            }
            else if (i == levelGraph.SrcArenaIndex) {
                arenaPrefab = srcArenaPrefab;
                sourceArena = true;
            }else {
                arenaPrefab = arenaPrefabs[levelGraph.GeneratedArenas[i].ArenaPrefabIndex];
            }
               
            arenaInstance = Instantiate(arenaPrefab, arenaLocation, rotation, gameObject.transform);

            if (sourceArena)
            {
                SetupPlayer();
            }

            arenaScript = arenaInstance.gameObject.GetComponent<Arena>();

            // set arena initial values (isSourceArena, hasCharacter, arenaLevel, availableDoorLocations)
            arenaScript.SetInitialValues(sourceArena, currLevel, levelGraph.GeneratedArenas[i].GetAvailableDoors);
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

                int rotValue = currNode.GetRotation();
                Quaternion rotation = Quaternion.identity;
                rotation.eulerAngles = new Vector3(0, rotValue, 0);

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

    public void SetupPlayer()
    {
        playerObj = GameObject.Find("Player");
        if (playerObj == null)
        {
            Debug.Log("Cannot find player GameObject");
            return;
        }

        PlayerMovement playerMovController = playerObj.GetComponent<PlayerMovement>();
        playerMovController.LevelLoad = true;
        playerMovController.InitLoc = levelGraph.CalculatePlayerInitLoc(levelGrid.PathWidth);
    }
}
