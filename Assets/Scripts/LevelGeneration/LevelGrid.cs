using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// Grid representation of level
    // gridScale value multiplies by values such as gridRows to get level gameobject dimensions 
public class LevelGrid
{
    private LevelData currLevel;
    // Important Note: for the current moment this code is dependent on gridRows + gridCols having odd values
    public int GridRows => currLevel.gridRows;
    public int GridCols => currLevel.gridCols;
    public int GridScale => currLevel.gridScale;

    private GridNode[,] levelLayout;
    private Vector2Int gridCenter;
    public Vector2Int GridCenter => gridCenter;

    // Random Walker Algorithm Attributes
    private bool isInitialPath; 
    private int maxWalkTries = 20000;

	private List<Vector2Int> startDoors, targetDoors;
    private Vector2Int currWalkerLoc, currStartLoc, currTargetLoc;
    private Dictionary<Vector2Int, (Vector2Int, Vector2Int)> currPath;

    public LevelGrid(LevelData level)
    {
        currLevel = level;

        levelLayout = new GridNode[currLevel.gridRows, currLevel.gridCols];
        InitializeNodes();

        int halfRowNum = (currLevel.gridRows - 1) / 2;
        int halfColNum = (currLevel.gridCols - 1) / 2;
        gridCenter = new Vector2Int(halfRowNum, halfColNum);
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
        int locX = Random.Range(minX, maxX);
        int locY = Random.Range(minY, maxY);
        return new Vector2Int(locX, locY);
    }

    // Add Arena Layout to Grid (after generating valid arenaCenter location on grid)
    // smallest arena size is 3x3
    public bool AddArena(Vector2Int centerLoc, int height, int width)
    {
        // Debug.Log("current center location: " + centerLoc);
        // Debug.Log("current height, width: " + height + " , " + width);

        int numRows = height / currLevel.gridScale;
        int numCols = width / currLevel.gridScale;

        int rowCenter = (numRows - 1) / 2;
        int colCenter = (numCols - 1) / 2;

        Vector2Int currLocation = new Vector2Int(0, 0);

        // loop through gridNodes setting appropriate grid nodes according to arena layout
        // error checking if arena is within grid boundaries
        // currently only for square arena -> procedurally set door locations at N,E,S,W positions relative to arenaCenter
        for(int i = -rowCenter; i <= rowCenter; i++){
            for(int j = -colCenter; j <= colCenter; j++){
                currLocation.Set((centerLoc.x + i), (centerLoc.y + j));
                if(IsValidTarget(currLocation)){
                    char node = 'X';
                    if((i == rowCenter | i == -rowCenter) & (j == 0)){
                        // doors at north and south of arena
                        node = 'D';
                    }else if((i == 0) & (j == -colCenter | j == colCenter)){
                        // doors at east and west of arena
                        node = 'D';
                    }else if((i == 0) & (j == 0)){
                        // set 'A' at arenaCenter
                        node = 'A';
                    }

                    levelLayout[currLocation.x, currLocation.y] = new GridNode(node);
                }else{
                    Debug.Log("Error: arena Location not within grid boundaries");
                    return false;
                }
            }
        }

        return true;
    }

    // After path is generated, add all path connections to grid
    // returns false if any errors occured while adding connection
    private bool AddPath()
    {
        foreach(KeyValuePair<Vector2Int, (Vector2Int, Vector2Int)> pathConnection in currPath){
            bool success = false;
            Vector2Int pathDir = pathConnection.Value.Item2;

            // Debug.Log("AddPath() : currLocation = " + pathConnection.Key + ", direction = " + pathDir);

            // add connection to curr node
            GridNode srcNode = GetNode(pathConnection.Key);
            success = srcNode.AddConnection(pathDir);
            if (!success){
                Debug.Log("Error adding pathConnection to GridNode");
                return false;
            }
            BlockNewCorners(pathConnection.Key);

            // add connection to target node
            GridNode targetNode = GetNode(pathConnection.Key + pathDir);
            success = targetNode.AddConnection(pathDir * (-1)); // connection from perspective of target node
            if (!success){
                return false;
            }

            BlockNewCorners(pathConnection.Key + pathDir);
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

        GridNode targetNode = new GridNode();
        targetNode = GetNode(targetLoc);

        if(targetNode.NodeValue == 'E' | targetNode.NodeValue == 'D')     // if node is empty or arenaDoor
            return true;
        else if(targetNode.NodeValue == 'X' | targetNode.NodeValue == 'A')    // if node is blocked, arenaCenter
            return false;
        else if(targetNode.NodeValue == 'I')   // if node is invalid (out of bounds of grid)
            return false;

        // initial path can't connecting to any other nodes (P, T, J) as they all indicate existing paths
        if(isInitialPath)
            return false;
        
        if(targetNode.NodeValue == 'P' | targetNode.NodeValue == 'T'){
            // adding connection to existing simple path or turn
            return true;
        }else if(targetNode.NodeValue == 'J'){
            // adding connection to existing junction
            if(targetNode.NumConnections == 4){
                Debug.Log("Error: trying to connect to junction that already has 4 connections");
                return false;
            }else{
                return true;             
            }
        }

        // if reached this point, something wrong - invalid value for targetNode
        Debug.Log("Error: CanConnect detected invalid value for targetNode");
        return false;
    }

    // helper function ensures corners of any turns/junctions are blocked
    private void BlockNewCorners(Vector2Int currLocation)
    {
        // locate currNode
        GridNode currNode = GetNode(currLocation);

        // if node is a turn, block only corner (only has 1)
        if(currNode.NodeValue == 'T'){
            GridNode cornerNode = GetNode(currNode.CurrCorners[0]);
            cornerNode.BlockCorner();
        }else if(currNode.NodeValue == 'J'){
            // if node is joint, block newest two corners
            GridNode cornerNode1, cornerNode2;

            if(currNode.NumConnections == 3){    // three way joint only has 2 corners
                cornerNode1 = GetNode(currNode.CurrCorners[0]);
                cornerNode2 = GetNode(currNode.CurrCorners[1]);
            }else if(currNode.NumConnections == 4){  // four way joint has 4 corners (only need to check newest two)
                cornerNode1 = GetNode(currNode.CurrCorners[2]);
                cornerNode2 = GetNode(currNode.CurrCorners[3]);
            }else{
                Debug.Log("Error: incorrect number of connections on node");
                cornerNode1 = new GridNode('I');
                cornerNode2 = new GridNode('I');
            }
            
            cornerNode1.BlockCorner();
            cornerNode2.BlockCorner();
        }
    }

    // modified variation of random walk algorithm
    public bool GeneratePath(bool initialPath, GraphNode startArena, GraphNode endArena){
        // first setup walker 
        isInitialPath = initialPath;
        
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

            currWalkerLoc = currStartLoc;
            currPath = new Dictionary<Vector2Int, (Vector2Int, Vector2Int)>();

            success = RandomWalker();

            if(success){
                AddPath();

                // mark used doors in src and target arena GraphNodes as unavailable
                if(!startArena.AddDoor(startDoors[startDoor]))
                    Debug.Log("Error adding door to start arena");
                
                if(!endArena.AddDoor(targetDoors[endDoor]))
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

        if(!success){
            if(isInitialPath){
                // error handling - if arena is unreachable from shortest path tree 
                Debug.Log("Error: no more possible door combinations, reached max number of path tries in walker algorithm");
            }else{
                // error handling
                Debug.Log("Error: arena unreachable (secondary walk)");
            }
        }

        return success;
    }

    private bool RandomWalker()
    {
        int numTries = 0;
        bool pathComplete = false;

        GridNode currNode = new GridNode();
        Vector2Int directionToTry = Vector2Int.zero;
        Vector2Int prevLocation = new Vector2Int(-1, -1);

        do
        {
            currNode = GetNode(currWalkerLoc);

            // if node hasn't been visited yet this path iteration
            if(!currPath.ContainsKey(currWalkerLoc))
                currNode.CalculateExplorationOrder(currWalkerLoc - currTargetLoc, currLevel.drunkenRatio);

            if(currNode.PathExplorationOrder.Count > 0){
                directionToTry = currNode.PathExplorationOrder.Dequeue();

                if(CanConnect(currWalkerLoc, directionToTry)){
                    currPath.Add(currWalkerLoc, (prevLocation, directionToTry));

                    prevLocation = currWalkerLoc;
                    currWalkerLoc += directionToTry;

                    if(currWalkerLoc == currTargetLoc)
                        pathComplete = true;
                }else{
                    numTries++;
                }
            }else{
                // if there are no more possible directions to explore in currNode
                numTries++;
                Vector2Int removeLoc = currWalkerLoc;

                // update prevDirection
                (Vector2Int prevLoc, Vector2Int tryDir) nodeToRemove;
                if(!currPath.TryGetValue(removeLoc, out nodeToRemove)){
                    Debug.Log("Error: nodeToRemove doesn't exist in currPath");
                    return false;
                }
                // set walker location back to prev node's location
                currWalkerLoc = nodeToRemove.prevLoc;

                // check if updated walkerLocation is back to start of path (end of walker algo, failure)
                if(currWalkerLoc == currStartLoc){
                    Debug.Log("no possible paths between curr doors");
                    return false;
                }else{
                    currPath.Remove(removeLoc);
                }

                // update prev location value
                (Vector2Int prevLoc, Vector2Int tryDir) newNode;
                if(!currPath.TryGetValue(currWalkerLoc, out newNode)){
                    Debug.Log("Error: currNode doesn't exist in currPath");
                    return false;
                }
                prevLocation = newNode.prevLoc;
            }
        } while (!pathComplete & (numTries < maxWalkTries));

        return pathComplete;
    }

    // helper function to convert location from vector2Int to Vector3 (for instantiation)
    public Vector3 ConvertLocation(int x, int y)
    {
        return new Vector3(10 * currLevel.gridScale * x , 0 , 10 * currLevel.gridScale * y);
    }

    public void PrintGrid(){
        for(int i = 0; i < currLevel.gridRows; i++){
            string row = levelLayout[i, 0].NodeValue.ToString();
            for(int j = 1; j < currLevel.gridCols; j++){
                row += levelLayout[i, j].NodeValue;
            }
            Debug.Log(row);
        }
    }
}
