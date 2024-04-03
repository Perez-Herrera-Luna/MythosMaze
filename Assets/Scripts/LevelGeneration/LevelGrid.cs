using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

// Grid representation of level
public class LevelGrid
{
    private LevelData currLevel;
    public int GridRows => currLevel.gridRows;
    public int GridCols => currLevel.gridCols;
    public int PathWidth => currLevel.pathWidth;

    private GridNode[,] levelLayout;
    private Vector2Int gridCenter;
    private float gridRadius;

    public Vector2Int GridCenter => gridCenter;
    public float GridRadius => gridRadius;

    // Random Walker Algorithm Attributes
    private bool isInitialPath;
    private bool pathComplete;
    private int maxWalkTries = 20000;

	private List<Vector2Int> startDoors, targetDoors;
    private Vector2Int currWalkerLoc, currStartLoc, currTargetLoc;
    private Dictionary<Vector2Int, (Vector2Int, Vector2Int)> currPath;
    private Stack<Vector2Int> currPathCorners;
    private int startArenaIndex;
    private bool reachedEndArena;

    public LevelGrid(LevelData level)
    {
        currLevel = level;

        levelLayout = new GridNode[currLevel.gridRows, currLevel.gridCols];
        InitializeNodes();

        int halfRowNum = (currLevel.gridRows - 1) / 2;
        int halfColNum = (currLevel.gridCols - 1) / 2;
        gridCenter = new Vector2Int(halfRowNum, halfColNum);
        gridRadius = (float)Math.Sqrt(currLevel.gridRows * currLevel.gridRows + currLevel.gridCols * currLevel.gridCols);
    }

    public void ResetGrid()
    {
        levelLayout = new GridNode[currLevel.gridRows, currLevel.gridCols];
        InitializeNodes();
    }

    private void InitializeNodes()
    {
        for(int i = 0; i < currLevel.gridRows; i++){
            for(int j = 0; j < currLevel.gridCols; j++){
                levelLayout[i, j] = new GridNode('E');
            }
        }
    }

    // helper function returns reference to node located at location vector
    // returns invalid node (nodeValue = 'I') if location is out of bounds
    public GridNode GetNode(Vector2Int location)
    {
        // check if location is valid
        if(IsValidTarget(location))
            return levelLayout[location.x, location.y];

        // otherwise, location invalid
        GridNode invalid = new GridNode('I');
        return invalid;
    }

    // helper function to check if location is within grid boundaries
    private bool IsValidTarget(Vector2Int location)
    {
        if(location.x >= 0 & location.x < currLevel.gridRows){
            if(location.y >= 0 & location.y < currLevel.gridCols)
                return true;
        }
        return false;
    }

    // generate random arena location within grid (leaving 1 gridwidth of space on each side)
    public Vector2Int GenerateArenaLocation(ArenaData arena)
    {
        int minX = (arena.width - 1) / 2;
        int maxX = currLevel.gridRows - minX;
        int minY = (arena.height - 1) / 2;
        int maxY = currLevel.gridCols - minY;

        // randomly generate center of arena (within smaller box constrained by arena proportions)
        int locX = ThreadSafeRandom.GetRandom(minX, maxX);
        int locY = ThreadSafeRandom.GetRandom(minY, maxY);

        return new Vector2Int(locX, locY);
    }

    // Add Arena Layout to Grid (after generating valid arenaCenter location on grid)
    // smallest arena size is 3x3
    public bool AddArena(Vector2Int centerLoc, int arenaIndex, ArenaData arena)
    {
        // Debug.Log("current center location: " + centerLoc);
        // Debug.Log("current height, width: " + arena.height + " , " + arena.width);

        int numRows = arena.height / PathWidth;
        int numCols = arena.width / PathWidth;

        float rowCenter = (numRows - 1) / 2.0f;
        float colCenter = (numCols - 1) / 2.0f;

        // Debug.Log("row/colC " + rowCenter + "," + colCenter);

        Vector2Int currLocation = new Vector2Int(0, 0);

        // loop through gridNodes setting appropriate grid nodes according to arena layout
        // error checking if arena is within grid boundaries
        // currently only for square arena -> procedurally set door locations at N,E,S,W positions relative to arenaCenter
        for(float i = -colCenter; i <= colCenter; i++){
            for(float j = -rowCenter; j <= rowCenter; j++){
                currLocation.Set((int)(centerLoc.x + i), (int)(centerLoc.y + j));

                // Debug.Log("adding arena node: " + currLocation);
                if(IsValidTarget(currLocation)){
                    char node = 'X';

                    if ((int)i == 0 & (int)j == 0)
                        node = 'A';     // set 'A' at arenaCenter

                    int roundedi = (i > 0) ? (int)i : (int)Math.Round(i, MidpointRounding.AwayFromZero);
                    int roundedj = (int)Math.Round(j, MidpointRounding.AwayFromZero);

                    foreach (Vector2Int door in arena.doorLocations)
                    {
                        if ((door.x == roundedi) & (door.y == roundedj))
                            node = 'D';
                    }

                    if(node == 'D')
                        levelLayout[currLocation.x, currLocation.y] = new GridNode(node, arenaIndex);
                    else
                        levelLayout[currLocation.x, currLocation.y] = new GridNode(node);
                }
                else{
                    Debug.Log("Error: arena Location not within grid boundaries");
                    return false;
                }
            }
        }

        return true;
    }

    private void RemoveArena(Vector2Int centerLoc, ArenaData arena)
    {
        int numRows = arena.height / PathWidth;
        int numCols = arena.width / PathWidth;

        float rowCenter = (numRows - 1) / 2.0f;
        float colCenter = (numCols - 1) / 2.0f;

        Vector2Int currLocation = new Vector2Int(0, 0);

        for (float i = -colCenter; i <= colCenter; i++)
        {
            for (float j = -rowCenter; j <= rowCenter; j++)
            {
                currLocation.Set((int)(centerLoc.x + i), (int)(centerLoc.y + j));

                if (IsValidTarget(currLocation))
                    levelLayout[currLocation.x, currLocation.y] = new GridNode('E');
            }
        }
    }

    public bool UpdateArena(Vector2Int centerLoc, int arenaIndex, ArenaData newArena, ArenaData oldArena)
    {
        RemoveArena(centerLoc, oldArena);

        return AddArena(centerLoc, arenaIndex, newArena);
    }

    // After path is generated, add all path connections to grid
    // returns false if any errors occured while adding connection
    private bool AddPath(int startArena, int endArena)
    {
        foreach(KeyValuePair<Vector2Int, (Vector2Int, Vector2Int)> pathConnection in currPath){
            bool success = false;
            Vector2Int pathDir = pathConnection.Value.Item2;

            //Debug.Log("AddPath() : currLocation = " + pathConnection.Key + ", direction = " + pathDir);

            // add connection to curr node
            GridNode srcNode = GetNode(pathConnection.Key);
            success = srcNode.AddConnection(pathDir, startArena, endArena, reachedEndArena);
            if (!success){
                Debug.Log("Error adding pathConnection to GridNode");
                return false;
            }

            // add connection to target node
            GridNode targetNode = GetNode(pathConnection.Key + pathDir);
            success = targetNode.AddConnection(pathDir * (-1), startArena, endArena, reachedEndArena); // connection from perspective of target node
            if (!success){
                Debug.Log("Error adding pathConnection to targetNode");
                return false;
            }

        }
        return true;
    }

    // function which returns true / false on whether adding a path connection in moveDirection is possible
    private bool CanConnect(Vector2Int currLoc, Vector2Int moveDir)
    {
        // set target node equal to walker's next location
        Vector2Int targetLoc = currLoc + moveDir;

        if(!IsValidTarget(targetLoc))
            return false;

        // check if path runs into itself
        if(currPath.ContainsKey(targetLoc))
            return false;

        // check if path is trying to connect to a newly added corner
        if (currPathCorners.Contains(targetLoc))
            return false;

        GridNode targetNode = new GridNode();
        targetNode = GetNode(targetLoc);

        // check if door belongs to the start arena
        if (targetNode.ArenaConnections.Contains(startArenaIndex))
            return false;

        if (targetNode.NodeValue == 'E' | targetNode.NodeValue == 'D')     // if node is empty or arena door
            return true;
        else if(targetNode.NodeValue == 'X' | targetNode.NodeValue == 'A')    // if node is blocked, arenaCenter
            return false;
        else if(targetNode.NodeValue == 'I')   // if node is invalid (out of bounds of grid)
            return false;

        // initial path can't connect to any other nodes (P, T, J) as they all indicate existing paths
        if(isInitialPath)
            return false;

        if(targetNode.NodeValue == 'P' | targetNode.NodeValue == 'T'){
            // check if simple path or turn already connects to the start arena
            if (targetNode.ArenaConnections.Contains(startArenaIndex))
                return false;
            else
                return true;
        }
        else if(targetNode.NodeValue == 'J'){
            // adding connection to existing junction
            if(targetNode.NumConnections == 4){
                Debug.Log("Error: trying to connect to junction that already has 4 connections");
                return false;
            }else{
                pathComplete = true;
                return true;             
            }
        }

        // if reached this point, something wrong - invalid value for targetNode
        Debug.Log("Error: CanConnect detected invalid value for targetNode");
        return false;
    }

    // modified variation of random walk algorithm
    public bool GeneratePath(bool initialPath, GraphNode startArena, int startIndex, GraphNode endArena, int endIndex){

        // Debug.Log("generating path btw: " + startArena.GridLocation + " and " +  endArena.GridLocation);

        // first setup walker 
        isInitialPath = initialPath;
        reachedEndArena = false;

        startArenaIndex = startIndex;
        startDoors = startArena.NearestDoors(endArena);
        if(startDoors == null){
            Debug.Log("No available doors at source arena");
            return false;
        }
        targetDoors = endArena.NearestDoors(startArena);
        if(targetDoors == null){
            Debug.Log("No available doors at end arena");
            return false;
        }

        bool success = false;
        int doorCombos = startDoors.Count * targetDoors.Count;
        int startDoor = 0, endDoor = 0;

        // tries all possible door combinations to connect path between 2 arenas
        while(!success & (doorCombos > 0)){

            currStartLoc = startArena.GridLocation + startDoors[startDoor];
            currTargetLoc = endArena.GridLocation + targetDoors[endDoor];

            // Debug.Log("generating path between: " + currStartLoc + " and " + currTargetLoc);

            currWalkerLoc = currStartLoc;
            currPath = new Dictionary<Vector2Int, (Vector2Int, Vector2Int)>();
            currPathCorners = new Stack<Vector2Int>();

            success = RandomWalker();
            reachedEndArena = (currWalkerLoc == currTargetLoc);

            if(success){
                if (!AddPath(startIndex, endIndex))
                    Debug.Log("Error adding path");

                // mark used doors in src and target arena GraphNodes as unavailable
                if(!startArena.AddDoor(startDoors[startDoor]))
                    Debug.Log("Error adding door to start arena");

                // check if path reached end arena, and if it did, add door to end arena
                if (reachedEndArena && !endArena.AddDoor(targetDoors[endDoor]))
                        Debug.Log("Error adding door to end arena");
            }else{
                doorCombos -= 1;
                // try different doors for startArena and endArena (if available)
                if(doorCombos > 0){
                    if(endDoor < (targetDoors.Count - 1)){
                        endDoor++;
                    }else{
                        startDoor++;
                        endDoor = 0;
                    }
                }
            }
        }

        /*if(!success){
            if(isInitialPath)
                Debug.Log("Error: arena unreachable (initial walk)");
        }*/

        return success;
    }

    private bool RandomWalker()
    {
        int numTries = 0;
        pathComplete = false;

        bool directionSuccess = true;
        GridNode currNode = new GridNode();
        Vector2Int directionToTry = Vector2Int.zero;
        Vector2Int prevLocation = new Vector2Int(-1, -1);

        do
        {
            currNode = GetNode(currWalkerLoc);

            if (!currPath.ContainsKey(currWalkerLoc) & directionSuccess)
            {
                // Debug.Log("visiting node: " + currWalkerLoc);
                currNode.CalculateExplorationOrder(currWalkerLoc - currTargetLoc, currLevel.drunkenRatio);
            }

            if (currNode.PathExplorationOrder.Count > 0)
            {
                directionSuccess = false;

                directionToTry = currNode.PathExplorationOrder.Dequeue();

                // Debug.Log("trying to connect from " + currWalkerLoc + " in direction " + directionToTry);

                if (CanConnect(currWalkerLoc, directionToTry))
                {
                    // Debug.Log("adding connection from " + currWalkerLoc + " in direction " + directionToTry);
                    directionSuccess = true;
                    currPath.Add(currWalkerLoc, (prevLocation, directionToTry));

                    GridNode targetNode = new GridNode();
                    targetNode = GetNode(currWalkerLoc + directionToTry);

                    // check if walker connected to an existing path
                    if (targetNode.NodeValue == 'P' | targetNode.NodeValue == 'T' | targetNode.NodeValue == 'J')
                        pathComplete = true;
                    else
                    {
                        // if the current node is becoming a Turn, add corners to pathCorner list
                        Vector2Int prevConnectDir = prevLocation - currWalkerLoc;
                        if (Vector2.Dot(directionToTry, prevConnectDir) == 0)
                            currPathCorners.Push(currWalkerLoc + directionToTry + prevConnectDir);

                        // increment walker
                        prevLocation = currWalkerLoc;
                        currWalkerLoc += directionToTry;

                        if (currWalkerLoc == currTargetLoc)
                            pathComplete = true;
                    }  
                }
                else
                {
                    numTries++;
                }
            }
            else {
                numTries++;
                directionSuccess = false;
                Vector2Int removeLoc = prevLocation;
                Vector2Int connectToRemoveDir = currWalkerLoc - prevLocation;

                // check if updated walkerLocation is back to start of path (end of walker algo, failure)
                if (currWalkerLoc == currStartLoc)
                {
                    return false;
                }
                else
                {
                    currWalkerLoc = prevLocation;

                    // update prev location value
                    (Vector2Int prevLoc, Vector2Int tryDir) newNode;
                    if (!currPath.TryGetValue(currWalkerLoc, out newNode))
                    {
                        Debug.Log("Error: currNode doesn't exist in currPath");
                        return false;
                    }
                    prevLocation = newNode.prevLoc;
                }

                currPath.Remove(removeLoc);

                // check if node to be removed had a blocked corner (is a turn from prevNode)
                Vector2Int prevConnectDir = prevLocation - currWalkerLoc;

                if (Vector2.Dot(connectToRemoveDir, prevConnectDir) == 0)
                    currPathCorners.Pop();
            }

        } while (!pathComplete & (numTries < maxWalkTries) & (currPath.Count < currLevel.maxPathLength));

        if (numTries == maxWalkTries)
            Debug.Log("error: reached max num walk tries");
        /*else if (pathComplete)
            Debug.Log("path complete");
        else
            Debug.Log("reached max path length");*/

        return pathComplete;
    }

    // helper function to convert location from vector2Int to Vector3 (for instantiation)
    public Vector3 ConvertLocation(int x, int y)
    {
        return new Vector3(10 * PathWidth * x , 0 , 10 * PathWidth * y);
    }

    public void PrintGrid(){
        for(int i = currLevel.gridCols - 1; i >= 0; i--){
            string row = levelLayout[i, 0].NodeValue.ToString();
            for(int j = 1; j < currLevel.gridRows; j++){
                row += levelLayout[j, i].NodeValue;
            }
            Debug.Log(row);
        }
    }
}
