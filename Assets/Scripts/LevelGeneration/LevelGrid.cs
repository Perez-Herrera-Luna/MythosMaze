using System;
using UnityEngine;

// Grid representation of level
    // gridScale value multiplies by values such as gridRows to get level gameobject dimensions 
public class LevelGrid
{
    // Important Note: for the current moment this code is dependent on gridRows + gridCols having odd values
    private int gridRows = 21;
    private int gridCols = 21;
    private int gridScale = 2;
    public int GridScale => gridScale;

    private GridNode[,] levelLayout;

    private Vector2Int gridCenter;
    public Vector2Int GridCenter => gridCenter;

    public LevelGrid()
    {
        levelLayout = new GridNode[gridRows, gridCols];

        int halfRowNum = (gridRows - 1) / 2;
        int halfColNum = (gridCols - 1) / 2;
        gridCenter = new Vector2Int(halfRowNum, halfColNum);
    }

    public LevelGrid(int rows, int cols)
    {
        gridRows = rows;
        gridCols = cols;

        levelLayout = new GridNode[gridRows, gridCols];

        int halfRowNum = (gridRows - 1) / 2;
        int halfColNum = (gridCols - 1) / 2;
        gridCenter = new Vector2Int(halfRowNum, halfColNum);
    }

    // helper function returns reference to node located at location vector
    // returns invalid node (nodeValue = 'I') if location is out of bounds
    private GridNode GetNode(Vector2Int location)
    {
        // check if location is valid
        if(isValidTarget(location))
            return levelLayout[location.x, location.y];

        // otherwise, location invalid
        return new GridNode('I');
    }

    // helper function to check if location is within grid boundaries
    private bool isValidTarget(Vector2Int location)
    {
        if(location.x >= 0 & location.x < gridRows){
            if(location.y >= 0 & location.y < gridCols)
                return true;
        }
        return false;
    }

    // generate random arena location within grid (leaving 1 gridwidth of space on each side)
    // check if location is valid for arena -> if not, retry (loop stops at success or maxAttemptsGenLoc)
    public bool GenerateArenaLocation(ArenaData arena)
    {
        // needs implementation still
        return false;
    }

    // Add Arena Layout to Grid (after generating valid arenaCenter location on grid)
    // smallest arena size is 3x3
    public bool AddArena(Vector2Int centerLoc, int height, int width)
    {
        int numRows = height / gridScale;
        int numCols = width / gridScale;

        int rowCenter = (numRows - 1) / 2;
        int colCenter = (numCols - 1) / 2;

        Vector2Int currLocation = new Vector2Int(0, 0);

        // loop through gridNodes setting appropriate grid nodes according to arena layout
        // error checking if arena is within grid boundaries
        // for square arena -> procedurally set door locations at N,E,S,W positions relative to arenaCenter
        for(int i = -rowCenter; i <= rowCenter; i++){
            for(int j = -colCenter; j <= colCenter; j++){
                currLocation.Set(i, j);
                if(isValidTarget(currLocation)){
                    if((i == rowCenter | i == -rowCenter) & (j == 0)){
                        // doors at north and south of arena
                        levelLayout[centerLoc.x + i, centerLoc.y + j].NodeValue = 'D';
                    }else if((i == 0) & (j == -colCenter | j == colCenter)){
                        // doors at east and west of arena
                        levelLayout[centerLoc.x + i, centerLoc.y + j].NodeValue = 'D';
                    }else if((i == 0) & (j == 0)){
                        // set 'A' at arenaCenter
                        levelLayout[centerLoc.x, centerLoc.y].NodeValue = 'A';
                    }else{
                        // set all other gridNodes within arena layout to X (blocked)
                        levelLayout[centerLoc.x + i, centerLoc.y + j].NodeValue = 'X';
                    }
                }else{
                    Debug.Log("Error: arena Location not within grid boundaries");
                    return false;
                }
            }
        }

        return true;
    }

    // function which returns true / false on whether adding a path connection in moveDirection is possible
    // may need additional checks, but for now I can't think of any
    public bool CanConnect(Vector2Int currLocation, Vector2Int moveDirection, bool isInitialPath)
    {
        // add any error checks of currNode

        // set target node equal to walker's next location
        GridNode targetNode = GetNode(currLocation + moveDirection);

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
            if(targetNode.NumConnections == 2){
                return true;
            }else{
                Debug.Log("Error: trying to connect to path with invalid NumConnections");
                return false;                
            }
        }else if(targetNode.NodeValue == 'J'){
            // adding connection to existing junction
            if(targetNode.NumConnections == 3){
                return true;
            }else if(targetNode.NumConnections == 4){
                Debug.Log("Error: trying to connect to junction that already has 4 connections");
                return false;
            }else{
                Debug.Log("Error: trying to connect to junction with invalid NumConnections");
                return false;                
            }
        }

        // if reached this point, something wrong - invalid value for targetNode
        Debug.Log("Error: CanConnect detected invalid value for targetNode");
        return false;
    }

    // Need to run CanConnect prior to calling this function
    // returns false if any errors occured while adding connection
    public bool AddConnection(Vector2Int currLocation, Vector2Int moveDirection){
        bool success = false;

        // add connection to curr node
        GridNode currNode = GetNode(currLocation);
        success = currNode.AddConnection(moveDirection);
        if(!success)
            return false;
        if(currNode.NodeValue == 'T' | currNode.NodeValue == 'J')
            BlockNewCorners(currLocation);

        // add connection to target node
        GridNode targetNode = GetNode(currLocation + moveDirection);
        success = targetNode.AddConnection(moveDirection * (-1)); // connection from perspective of target node
        if(!success)
            return false;

        BlockNewCorners(currLocation);

        return true;
    }

    // helper function ensures corners of any turns/junctions are blocked
    private void BlockNewCorners(Vector2Int currLocation)
    {
        // locate currNode
        GridNode currNode = GetNode(currLocation);

        // if node is a turn, block only corner (only has 1)
        if(currNode.NodeValue == 'T'){
            GridNode cornerNode = GetNode(currNode.currCorners[0]);
            cornerNode.BlockCorner();
        }else if(currNode.NodeValue == 'J'){
            // if node is joint, block newest two corners
            GridNode cornerNode1, cornerNode2;

            if(currNode.NumConnections == 3){    // three way joint only has 2 corners
                cornerNode1 = GetNode(currNode.currCorners[0]);
                cornerNode2 = GetNode(currNode.currCorners[1]);
            }else if(currNode.NumConnections == 4){  // four way joint has 4 corners (only need to check newest two)
                cornerNode1 = GetNode(currNode.currCorners[2]);
                cornerNode2 = GetNode(currNode.currCorners[3]);
            }else{
                Debug.Log("Error: incorrect number of connections on node");
                cornerNode1 = new GridNode('I');
                cornerNode2 = new GridNode('I');
            }
            
            cornerNode1.BlockCorner();
            cornerNode2.BlockCorner();
        }
    }

}
