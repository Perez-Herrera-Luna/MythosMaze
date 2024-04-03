using System.Collections.Generic;
using UnityEngine;

public class GridNode
{
    private char nodeValue = 'E'; // values can be: E, X, A, D, P, T, I
    private int numConnections = 0;
    private List<Vector2Int> currConnections = new List<Vector2Int>();   // stores Vector2Int.up, .down, .left, and .right
    private List<Vector2Int> currCorners = new List<Vector2Int>();
    private Queue<Vector2Int> pathExplorationOrder = new Queue<Vector2Int>();
    private HashSet<int> arenaConnections = new HashSet<int>();

    public GridNode() => nodeValue = 'I';

    public GridNode(char val)
    {
        if(val == 'E' |  val == 'X' | val == 'A')
        {
            nodeValue = val;
        }
        else
        {
            nodeValue = 'X';
        }
    }

    public GridNode(char val, int arena)
    {
        if(val == 'D')
        {
            nodeValue = val;
            arenaConnections.Add(arena);
        }
    }

    public char NodeValue => nodeValue;
    public int NumConnections => numConnections;
    public List<Vector2Int> CurrCorners => currCorners;
    public Queue<Vector2Int> PathExplorationOrder => pathExplorationOrder;
    public HashSet<int> ArenaConnections => arenaConnections;

    // function which adds path connection in given direction to currNode
    // returns true if successful, false if unable to add connection
    public bool AddConnection(Vector2Int newConnection, int startArena, int endArena, bool reachedEndArena){

        arenaConnections.Add(startArena);

        if (reachedEndArena) 
            arenaConnections.Add(endArena);

        // if node is currently empty, add connection, change node to path
        if (nodeValue == 'E')
        {
            numConnections++;
            currConnections.Add(newConnection);
            nodeValue = 'P';
            return true;
        }

        // if node is a door, check if it is available
        if (nodeValue == 'D')
        {
            if (numConnections == 0)
            {
                numConnections++;
                currConnections.Add(newConnection);
                return true;
            }
            else
            {
                return false;
            }
        }

        // if node is currently path, check if path becomes simple path, turn, or junction
        if(nodeValue == 'P'){
            // if path currently is connected only on one end (becomes simple path or turn)
            if(numConnections == 1){
                // check if new connection is parallel or perpendicular to currConnection
                float dotProduct = Vector2.Dot(currConnections[0], newConnection);

                if(dotProduct == -1){
                    // connections are parallel, node becomes Simple Path
                    numConnections++;
                    currConnections.Add(newConnection);
                    return true;
                }else if(dotProduct == 0){
                    // connections are perpendicular, node becomes Turn
                    nodeValue = 'T';
                    numConnections++;
                    currConnections.Add(newConnection);
                    // add new corner to corner list
                    currCorners.Add(currConnections[0] + newConnection);
                    return true;
                }else if(dotProduct == 1){
                    // new connection is the same as existing connection = no changes
                    return true; 
                }else{
                    // invalid dot product result - smtg is seriously wrong (vectors probably not normalized)
                    Debug.Log("Error: invalid dotProduct value when adding path connection");
                    return false; 
                }

             // if path already has two connections (becomes junction)
            }else if(numConnections == 2){
                // changing simple path into 3-way junction
                nodeValue = 'J';
                numConnections++;
                currConnections.Add(newConnection);       
                // add 2 new corner locations to list (new path is perpendicular to both existing connections)
                currCorners.Add(currConnections[0] + newConnection);
                currCorners.Add(currConnections[1] + newConnection);
                return true;
            }else{
                Debug.Log("Error: trying to connect to path with invalid numConnections");
                return false;   
            }
        }

        // if node is currently turn, change to junction
        if (nodeValue == 'T'){
            if(numConnections == 2){
                nodeValue = 'J';
                numConnections++;
                currConnections.Add(newConnection);

                // add appropriate corner to list
                if (Vector2.Dot(currConnections[0], newConnection) == 0){
                    currCorners.Add(currConnections[0] + newConnection);
                }else if(Vector2.Dot(currConnections[1], newConnection) == 0){
                    currCorners.Add(currConnections[1] + newConnection);
                }else{
                    Debug.Log("Error: invalid dotproduct value");
                    return false;
                }
                return true;
            }else{
                Debug.Log("Error: trying to connect to turn with invalid numConnections");
                return false;
            }
        }

        // if node is currently 3-way junction, change to 4-way junction
        if (nodeValue == 'J'){
            // adding connection to 3-way joint
            if(numConnections == 3){
                numConnections++;
                currConnections.Add(newConnection);

                // add new corners to list (correspond to perpendicular path connections)
                foreach (Vector2Int currConnection in currConnections)
                {
                    if (Vector2.Dot(currConnection, newConnection) == 0)
                    {
                        currCorners.Add(currConnection + newConnection);
                    }
                }

                return true;
            }else{
                Debug.Log("Error: trying to connect to junction with invalid numConnections");
                return false;
            }
        }

        // if reached here, error
        Debug.Log("Error: trying to connect to invalid nodeType: " + nodeValue);
        return false;   
    }

    // generate order of path direction
    public void CalculateExplorationOrder(Vector2Int targetDir, int drunkenRatio)
    {
        pathExplorationOrder.Clear();

        // check if target is nearby
        if (targetDir.magnitude < 3)
        {
            BiasedOrder(targetDir);
        }
        else
        {
            int probability = ThreadSafeRandom.GetRandom(0, drunkenRatio);

            if (probability == 0)
            {   // chance of occuring = 1 / drunkenRatio
                RandomOrder();
            }
            else
            {
                BiasedOrder(targetDir);
            }
        }

    }

    // Generate order (N E S W) of path directions to try using random generation
    private void RandomOrder()
    {
        List<Vector2Int> pathDirections = new List<Vector2Int>            
            { new Vector2Int(0, 1), new Vector2Int(0, -1),
             new Vector2Int(1, 0), new Vector2Int(-1, 0)};
        
        // randomly shuffle pathDirections using Fisher-Yates Shuffle Algorithm (modern version)
        for(int i = 3; i >= 0; i--){
            int j = ThreadSafeRandom.GetRandom(0, i+1); // int overload of random range [minInclusive, maxExclusive)

            // perform shuffle
            Vector2Int direction1 = pathDirections[j];
            pathDirections[j] = pathDirections[i];
            pathDirections[i] = direction1;
        }

        foreach(Vector2Int dir in pathDirections){
            pathExplorationOrder.Enqueue(dir);
        }
    }

    // Generate order (N E S W) of path directions to try using random generation w/ direction Bias
    private void BiasedOrder(Vector2Int targetDir)
    {
        List<Vector2Int> pathDirections = new List<Vector2Int>            
            { new Vector2Int(0, 1), new Vector2Int(0, -1),
             new Vector2Int(1, 0), new Vector2Int(-1, 0)};
        
        SortedList<float, Vector2Int> sortedPaths = new SortedList<float, Vector2Int>();

        // finds paths nearest targetDir (largest dot product)
        foreach (Vector2Int path in pathDirections)
        {
            float currDotProduct = Vector2.Dot(path, targetDir);

            // make dot product 'unique' if same as another path
            if (sortedPaths.ContainsKey(currDotProduct))
                currDotProduct += 0.1f;

            sortedPaths.Add(currDotProduct, path);
        }

        foreach(Vector2Int dir in sortedPaths.Values){
            pathExplorationOrder.Enqueue(dir);
        }
    }

    public int GetRotation(){
        int rot = 0;
        if(nodeValue == 'P'){
            // if node is path, compare one of connections with vector UP to determine orientation
            float dotProduct = Vector2.Dot(Vector2Int.up, currConnections[0]);

            if (dotProduct == 0)
                rot = 90;
        }else if(nodeValue == 'T'){
            Vector2Int cornerDirection = currConnections[0] + currConnections[1];
            Vector2Int turnPrefabDir = new Vector2Int(1, -1);

            // compare cornerDirection with turnPrefabDir <1,-1>
            float dotProduct = Vector2.Dot(turnPrefabDir, cornerDirection);

            if (dotProduct == 0){
                // corner direction is either <1, 1> => rotate(270) OR <-1, -1> => rotate(90)
                turnPrefabDir.Set(-1, -1);

                if(cornerDirection == turnPrefabDir)
                    rot = 90;
                else
                    rot = 270;
            }else if(dotProduct == -2){
                // corner direction = <-1, 1>, rotate 180 degrees
                rot = 180;
            }
            // otherwise corner direction = <1, -1> = turnPrefabDir
        }else if(nodeValue == 'J'){
            if(numConnections == 3){
                Vector2Int junctionDirection = currConnections[0] + currConnections[1] + currConnections[2];
                Vector2Int j3PrefabDir = new Vector2Int(1, 0);

                // compare 3-way junction direction with <1, 0>
                float dotProduct = Vector2.Dot(j3PrefabDir, junctionDirection);

                if (dotProduct == 0){
                    // 3-way junction direction is either <0, -1> (rot = 90) or <0, 1> (rot = -90)
                    j3PrefabDir.Set(0, -1);

                    if(junctionDirection == j3PrefabDir)
                        rot = 90;
                    else
                        rot = 270;
                }else if(dotProduct == -1){
                    // 3-way junction direction = <-1, 0>, rotate 180 degrees
                    rot = 180;
                }
                // otherwise 3-way junction direction = <1, 0> = j3PrefabDir
            }
            // four-way junctions just have rot = Quaternion.identity, thus no need to adjust rot
        }
        return rot;
    }
}
