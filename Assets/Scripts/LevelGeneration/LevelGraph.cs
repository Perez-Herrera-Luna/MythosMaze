using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Graph representation of level
// Arenas are nodes, each node contains edges to all other arenas
public class LevelGraph
{
    private LevelData currLevel;
    // Graph Attributes (list of nodes + adjacency list)
    private int numArenasAdded = 0;
    public int  NumArenasAdded => numArenasAdded;

    private List<GraphNode> generatedArenas = new List<GraphNode>();
    public List<GraphNode> GeneratedArenas => generatedArenas; 
    private List<(int, float)>[] arenaAdjacencyList;    // array of Lists
    private float maxCollisionRadius = 0;

    // Dijkstra's Algorithm Attributes  
    private int srcArenaIndex;       // arena furthest distance from boss
    public int SrcArenaIndex => srcArenaIndex;
    private Dictionary<int, int> shortestPath;   // shortestPath (index, prevNodeIndex)
    public Dictionary<int, int> ShortestPath => shortestPath;
    private bool[] visited;
    private NodePriorityQueue nodePriorityQueue;    // priority queue with minDistances to boss

    public LevelGraph(LevelData level)
    {
        currLevel = level;
        // dijkstra's algo setup
        shortestPath = new Dictionary<int, int>();
        nodePriorityQueue = new NodePriorityQueue();
        arenaAdjacencyList = new List<(int, float)>[currLevel.maxNumArenas];
        visited = new bool[currLevel.maxNumArenas];
    }

    // Try adding new arena node to graph (compare location with already generated arenas)
    // called from ProceduralLevel
    public bool AddArena(ArenaData arena, Vector2Int newLoc, int gridScale)
    {
        // check if boss arena (1st arena to be added)
        if(numArenasAdded == 0){

            // create graphNode with distToBoss = 0 and only has 1 door
            GraphNode currNode = new GraphNode(0, 1);
            // set initial arena data in graphNode
            currNode.SetInitialNodeValues(newLoc, arena, gridScale);
            generatedArenas.Add(currNode);
            
            // initially, boss arena has no connections ( no other arenas generated )
            arenaAdjacencyList[0] = new List<(int, float)>();

            maxCollisionRadius = arena.collisionRadius;

            // if everything worked correctly
            numArenasAdded++;
            return true;
        }

        // otherwise (i.e. arena is not boss arena)
        List<(int, float)> adjArenas = new List<(int, float)>();
        float distance = 0, distToBoss = float.MaxValue;
        int existingArenaIndex = 0;
        int numDoors = 0;

        // check if new arena location is far enough away from already generated arenas
        foreach(GraphNode existingArena in generatedArenas){
            // calculate distance between generated grid location and existing arenas
            distance = Vector2.Distance(newLoc, existingArena.GridLocation);

            // basic check comparing 'radius' of arena w/ maxCollisionRadius
            if(distance < arena.collisionRadius + maxCollisionRadius + currLevel.arenaDistanceBuff){
                // large likelihood of overlap between arena and already generated arenas
                return false;
            }

            distance = distance * 2; // buff distance

            // if arena just checked is the boss arena (index 0)
            if(existingArenaIndex == 0){
                // buff distance to boss arena
                distance *= 4;
                distToBoss = distance;
            }

            // add generated arena to curr list of adjacent arenas
            adjArenas.Add((existingArenaIndex, distance));
            existingArenaIndex++;
        }

        // if reached here, then arena location is far enough from all previously generated arenas
        // can therefore add arena to generatedArenas, arenaAdjacencyList

        // add edge to existing arenas connecting newly added arena with distance just calculated
        foreach((int index, float dist) connection in adjArenas){
            // numArenas, since incremented at the end of AddArena(), currently represents index of new arena
            arenaAdjacencyList[connection.index].Add((numArenasAdded, connection.dist));
        }

        // (1) calculate num doors based on distToBoss
        if(distToBoss < (currLevel.gridRings[0] * currLevel.gridRings[0]))    // arenas closest to boss 
            numDoors = 4;
        else if(distToBoss < (currLevel.gridRings[1] * currLevel.gridRings[1]))
            numDoors = 3;
        else
            numDoors = 2;   // all other arenas must have at least 2 doors
        

        // set Graph node for arena with appropriate parameters
        GraphNode newNode = new GraphNode(distToBoss, numDoors);
        newNode.SetInitialNodeValues(newLoc, arena, gridScale);

        generatedArenas.Add(newNode);
        arenaAdjacencyList[numArenasAdded] = adjArenas;
        maxCollisionRadius = arena.collisionRadius > maxCollisionRadius ? arena.collisionRadius : maxCollisionRadius;

        numArenasAdded++;
        return true;
    }

    // public function called by proceduralLevel to generate shortest path
    public void GenerateShortestPathTree()
    {
        // PrintArenaAdjacencyList();

        // compute srcArena index = index of arena furthest distance from gridCenter (bossArena)
        float largestDist = 0;
        for(int i = 0; i < numArenasAdded; i++){
            float arenaDist = generatedArenas[i].DistToBoss;

            if(arenaDist > largestDist){
                srcArenaIndex = i;
                largestDist = arenaDist;
            }
        }

        // initialize dijkstra algorithm helper attributes

        // reset shortest path values
        shortestPath.Clear();
        shortestPath[srcArenaIndex] = -1;   

        // add each arena node to priority queue, set distances at "infinity"
        // set source node distance at 0
        for(int i = 0; i < numArenasAdded; i++){
            visited[i] = false;

            if(i == srcArenaIndex)
                nodePriorityQueue.AddNode(i, 0);
            else
                nodePriorityQueue.AddNode(i, float.MaxValue);
        }

        // call dijkstra's algorithm - populates shortestPath tree
        Dijkstras();

        PrintShortestPath();
    }

    // Dijkstras algorithm to compute shortest path tree
    // modified so it prioritizes next node according to their distance from center of grid (boss)
    private void Dijkstras()
    {
        (int index, float distance) currNode;

        while(!nodePriorityQueue.IsEmpty()){
            currNode = nodePriorityQueue.DequeueSmallest();
            visited[currNode.index] = true;

            // Debug.Log("currNode: " + currNode.index);

            // for each node adjacent to currently visiting one, calculate new path distance (through currNode)
            foreach ((int index, float distance) adjNode in arenaAdjacencyList[currNode.index])
            {
                if(!visited[adjNode.index]){
                    float currDistance = nodePriorityQueue.GetDistance(adjNode.index);

                    if(currDistance != -1){
                        float newDistance = currNode.distance + adjNode.distance;

                        // Debug.Log("currDist: " + currDistance + ", newDist: " + newDistance);

                        // if new path distance is smaller than curr distance to node, add node to path
                        if (newDistance < currDistance)
                        {
                            // Debug.Log("newDistance < currDistance");
                            nodePriorityQueue.UpdateDistance(adjNode.index, newDistance);

                            // update shortest path for adj node (prevNode == currNode)
                            shortestPath[adjNode.index] = currNode.index;
                        }
                    }
                }
            }
        }
    }

    public void PrintShortestPath()
    {
        Debug.Log("Shortest path tree: ");
        Dictionary<int, int>.KeyCollection keys = shortestPath.Keys;

        foreach(int index in keys){
            Debug.Log("node = " + index + ", prevNode = " + shortestPath[index]);
        }
    }

    public void PrintArenaAdjacencyList()
    {
        for(int i = 0; i < numArenasAdded; i++){
            for(int j = 0; j < arenaAdjacencyList[i].Count; j++){
                Debug.Log("index: " + i + ", adjNode: " + arenaAdjacencyList[i][j].Item1 + ", dist: " + arenaAdjacencyList[i][j].Item2);
            }
        }
    }
}