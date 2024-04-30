using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Graph representation of level
// Arenas are nodes, each node contains edges to all other arenas
public class LevelGraph
{
    [Header("Current Level & Arena Data")]
    private LevelData currLevel;
    private ArenaData srcArenaData;

    [Header("Graph Attributes")]
    private int numArenasAdded = 0;
    private List<GraphNode> generatedArenas = new List<GraphNode>();
    private List<(int, float)>[] arenaAdjacencyList;    // array of Lists
    private float maxCollisionRadius = 0;

    public int NumArenasAdded => numArenasAdded;
    public List<GraphNode> GeneratedArenas => generatedArenas; 
    public GraphNode GetBossArena => generatedArenas[0];
    public float MaxCollisionRadius => maxCollisionRadius;

    [Header("Dijkstra's Algorithm Attributes")]
    private NodePriorityQueue nodePriorityQueue;    // priority queue with minDistances to boss
    private int srcArenaIndex;       // arena furthest distance from boss
    private bool[] visited;

    private Dictionary<int, int> shortestPath;   // shortestPath (index, prevNodeIndex)

    public int SrcArenaIndex => srcArenaIndex;
    public Dictionary<int, int> ShortestPath => shortestPath;

    // Public Methods [called by ProceduralLevel
    public LevelGraph(LevelData level, ArenaData sourcArenaData)
    {
        currLevel = level;
        srcArenaData = sourcArenaData;
        // dijkstra's algo setup
        shortestPath = new Dictionary<int, int>();
        nodePriorityQueue = new NodePriorityQueue();
        arenaAdjacencyList = new List<(int, float)>[currLevel.maxNumArenas];
        visited = new bool[currLevel.maxNumArenas];
    }

    public void ResetGraph()
    {
        shortestPath = new Dictionary<int, int>();
        nodePriorityQueue = new NodePriorityQueue();
        arenaAdjacencyList = new List<(int, float)>[currLevel.maxNumArenas];
        visited = new bool[currLevel.maxNumArenas];

        generatedArenas = new List<GraphNode>();
        maxCollisionRadius = 0;
        numArenasAdded = 0;
    }

    // Try adding new arena location to graph (compare location with already generated arenas)
    // called from ProceduralLevel
    public bool AddArena(ArenaData arena, int arenaPrefabIndex, Vector2Int newLoc, int pathWidth, float gridRadius)
    {
        // check if boss arena (1st arena to be added)
        if(numArenasAdded == 0){

            if (!arena.isBossArena)
            {
                Debug.Log("Error: trying to assign non-boss arena as boss arena");
            }

            // create graphNode with distToBoss = 0, maxNumDoors = 1
            GraphNode currNode = new GraphNode(0, 1);
            // set initial arena data in graphNode
            currNode.SetInitialNodeValues(newLoc, arena, pathWidth, -1);
            generatedArenas.Add(currNode);
            
            // initially, boss arena has no connections ( no other arenas generated )
            arenaAdjacencyList[0] = new List<(int, float)>();

            maxCollisionRadius = arena.collisionRadius;
            numArenasAdded++;
        }
        else
        {
            List<(int, float)> adjArenas = new List<(int, float)>();
            float distance = 0, distToBoss = float.MaxValue;
            int existingArenaIndex = 0;

            // check if new arena location is far enough away from already generated arenas
            foreach (GraphNode existingArena in generatedArenas)
            {
                // calculate distance between generated grid location and existing arenas
                distance = Vector2.Distance(newLoc, existingArena.GridLocation);

                // basic check comparing 'radius' of arena w/ maxCollisionRadius
                if (distance < arena.collisionRadius + maxCollisionRadius + currLevel.arenaDistanceBuff)
                {
                    // large likelihood of overlap between arena and already generated arenas
                    return false;
                }

                distance = distance * 2; // buff distance

                // if arena just checked is the boss arena (index 0)
                if (existingArenaIndex == 0)
                {
                    // buff distance to boss arena
                    distance *= 4;
                    distToBoss = distance;
                }

                // add generated arena to curr list of adjacent arenas
                adjArenas.Add((existingArenaIndex, distance));
                existingArenaIndex++;
            }

            // add edge to existing arenas connecting newly added arena with distance just calculated
            foreach ((int index, float dist) connection in adjArenas)
            {
                // numArenas, since incremented at the end of AddArena(), currently represents index of new arena
                arenaAdjacencyList[connection.index].Add((numArenasAdded, connection.dist));
            }

            // if collision was detected
            if (distToBoss == -1)
                return false;

            // buff gridRadius (same amount as buff arena distToBoss above
            gridRadius *= 8;

            // Debug.Log("adding arena: " + numArenasAdded + " w/ distToBoss, loc = " + distToBoss + ", " + newLoc);

            // set Graph node for arena with appropriate parameters
            GraphNode newNode = new GraphNode(distToBoss, -1);
            newNode.SetInitialNodeValues(newLoc, arena, pathWidth, arenaPrefabIndex);

            generatedArenas.Add(newNode);
            arenaAdjacencyList[numArenasAdded] = adjArenas;
            maxCollisionRadius = arena.collisionRadius > maxCollisionRadius ? arena.collisionRadius : maxCollisionRadius;

            numArenasAdded++;
        }

        return true;
    }

    // returns true if no collision detected
    private bool CheckForArenaCollision(ArenaData arena, Vector2Int arenaLoc)
    {
        float distance = 0;

        // check if new arena location is far enough away from already generated arenas
        foreach (GraphNode existingArena in generatedArenas)
        {
            // calculate distance between generated grid location and existing arenas
            distance = Vector2.Distance(arenaLoc, existingArena.GridLocation);

            if(distance != 0)
            {
                // basic check comparing 'radius' of arena w/ maxCollisionRadius
                if (distance < arena.collisionRadius + maxCollisionRadius + currLevel.arenaDistanceBuff)
                {
                    // large likelihood of overlap between arena and already generated arenas
                    return false;
                }
            }
        }

        return true;
    }

    public bool CalculateSourceArenaIndex(int pathWidth)
    {
        // compute srcArena index = index of arena furthest distance from gridCenter (bossArena)
        float largestDist = 0;
        for (int i = 0; i < numArenasAdded; i++)
        {
            float arenaDist = generatedArenas[i].DistToBoss;

            if (arenaDist > largestDist)
            {
                srcArenaIndex = i;
                largestDist = arenaDist;
            }
        }

        if (CheckForArenaCollision(srcArenaData, generatedArenas[srcArenaIndex].GridLocation))
            return true;
        else
            return false;
    }

    public void UpdateSourceArena(ArenaData newArena, int pathWidth)
    {
        generatedArenas[srcArenaIndex].UpdateArenaDataValue(0, newArena, pathWidth);
    }

    // public function called by proceduralLevel to generate shortest path
    public void GenerateShortestPathTree()
    {
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

        // PrintShortestPath();
    }

    // Dijkstras algorithm to compute shortest path tree
    // modified so it prioritizes next node according to their distance from center of grid (boss)
    private void Dijkstras()
    {
        (int index, float distance) currNode;

        while(!nodePriorityQueue.IsEmpty()){
            currNode = nodePriorityQueue.DequeueSmallest();
            visited[currNode.index] = true;


            // for each node adjacent to currently visiting one, calculate new path distance (through currNode)
            foreach ((int index, float distance) adjNode in arenaAdjacencyList[currNode.index])
            {
                if(!visited[adjNode.index]){
                    float currDistance = nodePriorityQueue.GetDistance(adjNode.index);

                    if(currDistance != -1){
                        float newDistance = currNode.distance + adjNode.distance;

                        // if new path distance is smaller than curr distance to node, add node to path
                        if (newDistance < currDistance)
                        {
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

    public Vector3 CalculatePlayerInitLoc(int pathWidth)
    {
        return generatedArenas[srcArenaIndex].PlayerInitLoc(pathWidth);
    }
}