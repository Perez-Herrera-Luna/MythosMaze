using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Graph representation of level
// Arenas are nodes, each node contains edges to all other arenas
public class LevelGraph
{
    public LevelData currLevel;
    // Graph Attributes (list of nodes + adjacency list)
    private int numArenas;
    public int NumArenas => numArenas;

    public List<GraphNode> generatedArenas = new List<GraphNode>();
    private List<(int, float)>[] arenaAdjacencyList;    // array of Lists
    float maxCollisionRadius = 0;

    // Dijkstra's Algorithm Attributes  
    private int srcArenaIndex;       // arena furthest distance from boss
    private bool[] visited;
    private float[] minDistances;
    private List<int>[] shortestPath;   // array of Lists (shortest path tree)

    // priorityQueue chooses smallestPriority thus priority = -distToBoss
    private List<(int, float)> nodeQueue = new List<(int, float)>();

    public LevelGraph(LevelData level)
    {
        currLevel = level;
        // dijkstra's algo setup
        visited = new bool[currLevel.maxNumArenas];
        minDistances = new float[currLevel.maxNumArenas];
        shortestPath = new List<int>[currLevel.maxNumArenas];
        arenaAdjacencyList = new List<(int, float)>[currLevel.maxNumArenas];
    }

    // Try adding new arena node to graph (compare location with already generated arenas)
    public bool AddArena(ArenaData arena, Vector2Int newLoc, int gridScale)
    {
        // check if boss arena (1st arena to be added)
        if(numArenas == 0){

            // create graphNode with distToBoss = 0 and only has 1 door
            GraphNode currNode = new GraphNode(0, 1);
            // set initial arena data in graphNode
            currNode.SetInitialNodeValues(newLoc, arena, gridScale);
            generatedArenas.Add(currNode);
            
            // initially, boss arena has no connections ( no other arenas generated )
            arenaAdjacencyList[0] = new List<(int, float)>();

            maxCollisionRadius = arena.collisionRadius;

            // if everything worked correctly
            numArenas++;
            return true;
        }

        // otherwise (i.e. arena is not boss arena)
        List<(int, float)> adjArenas = new List<(int, float)>();
        float distance = 0, distToBoss = float.MaxValue;
        int existingArenaNum = 0;

        // check if new arena location is far enough away from already generated arenas
        foreach(GraphNode existingArena in generatedArenas){
            // calculate distance between generated grid location and existing arenas
            distance = Vector2.Distance(newLoc, existingArena.GridLocation);

            // basic check comparing 'radius' of arena w/ maxCollisionRadius
            if(distance < arena.collisionRadius + maxCollisionRadius){
                // large likelihood of overlap between arena and already generated arenas
                return false;
            }

            // if arena just checked is the boss arena (index 0)
            if(existingArenaNum == 0){
                distToBoss = distance;
            }

            // add generated arena to curr list of adjacent arenas
            adjArenas.Add((existingArenaNum, distance));
            existingArenaNum++;
        }

        // if reached here, then arena location is far enough from all previously generated arenas

        // add edge to existing arenas connecting newly added arena with distance just calculated
        foreach((int index, float dist) connection in adjArenas){
            // numArenas, since incremented at the end of AddArena(), curr represents index of new arena
            arenaAdjacencyList[connection.index].Add((numArenas, connection.dist));
        }

        // (1) calculate num doors based on distToBoss
        int numDoors = 0;
        if(distToBoss < currLevel.gridRings[0]){    // arenas closest to boss 
            numDoors = Random.Range(3, 5);  // randomly chooses 3 or 4 doors
        }else if(distToBoss < currLevel.gridRings[1]){
            numDoors = Random.Range(2, 4);  // randomly chooses 2 or 3 doors
        }else{
            numDoors = 2;   // all other arenas must have at least 2 doors
        }

        // set Graph node for arena with appropriate parameters
        GraphNode newNode = new GraphNode(distToBoss, numDoors);
        newNode.SetInitialNodeValues(newLoc, arena, gridScale);

        generatedArenas.Add(newNode);
        arenaAdjacencyList[numArenas] = adjArenas;
        maxCollisionRadius = arena.collisionRadius > maxCollisionRadius ? arena.collisionRadius : maxCollisionRadius;

        numArenas++;
        return true;
    }

    // public function called by proceduralLevel to generate shortest path
    public void GenerateShortestPathTree()
    {
        // modify edge weights as desired prior to running shortest path algorithm

        // compute srcArena index (if not already done during arena generation) :
            // furthest distance from gridCenter (bossArena)

        // initialize dijkstra algorithm helper attributes
        for(int i = 0; i < numArenas; i++){
            // add each arena node to priority queue based on squared distance to boss
            float bossPriority = (-1) * generatedArenas[i].distToBoss * generatedArenas[i].distToBoss;
            nodeQueue.Add((i, bossPriority));

            // mark each arena node as unvisited
            visited[i] = false;

            //set initial minDistances to src node at "infinity"
            minDistances[i] = int.MaxValue;
        }

        // set source node as first in priority queue (furthest dist from boss)
        srcArenaIndex = queueSmallestIndex();

        // set source arena distance to 0 and set it's shortest path to null
        minDistances[srcArenaIndex] = 0;
        shortestPath[srcArenaIndex] = null;

        // call dijkstra's algorithm - populates shortestPath tree
        Dijkstras();
    }

    // Dijkstras algorithm to compute shortest path tree
    // modified so it prioritizes next node according to their distance from center of grid (boss)
    public void Dijkstras()
    {
        (int index, float priority) currNode;

        while(!isQueueEmpty()){
            currNode = DequeueSmallest();

            // only perform actions if node hasn't been visited yet
            if(!visited[currNode.index]){
                visited[currNode.index] = true;

                // for each node adjacent to currently visiting one, calculate new path distance (through currNode)
                foreach((int index, float dist) adjNode in arenaAdjacencyList[currNode.index]){
                    float newDistance = minDistances[currNode.index] + adjNode.dist;
                
                    // if new path distance is smaller than curr distance to node, add node to path
                    if(newDistance < minDistances[adjNode.index]){
                        minDistances[adjNode.index] = newDistance;

                        // update shortest path for adj node to: path to currNode + currNode
                        shortestPath[adjNode.index] = shortestPath[currNode.index];
                        shortestPath[adjNode.index].Add(currNode.index);
                    }
                }
            }
        }
    }

    // makeshift priority queue helper functions (for now)
    public bool isQueueEmpty() => nodeQueue.Count > 0;

    // Dequeue smallest from nodeQueue (according to priority float)
    public (int, float) DequeueSmallest()
    {
        var smallest = nodeQueue[0];
        for(int i = 1; i < numArenas; i++){
            if(nodeQueue[i].Item2 < smallest.Item2){
                smallest = nodeQueue[i];
            }
        }

        nodeQueue.Remove(smallest);
        return smallest;
    }

    // returns index of arena with smallest priority
    public int queueSmallestIndex()
    {
        (int index, float dist) smallest = nodeQueue[0];
        for(int i = 1; i < numArenas; i++){
            if(nodeQueue[i].Item2 < smallest.Item2){
                smallest = nodeQueue[i];
            }
        }
        return smallest.index;
    }
}