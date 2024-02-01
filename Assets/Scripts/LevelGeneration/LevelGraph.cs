using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Graph representation of level
// Arenas are nodes, each node contains edges to all other arenas
public class LevelGraph
{
    // Graph Attributes (list of nodes + adjacency list)
    private int numArenas;
    private List<GraphNode> generatedArenas;
    private List<(int, int)>[] arenaAdjacencyList;

    // Dijkstra's Algorithm Attributes  
    private int srcArena;       // arena furthest distance from boss
    private List<int> notVisited;
    private bool[] visited;
    // priorityQueue chooses smallestPriority thus priority = -distToBoss
    private List<(int, int)> nodeQueue;     // (index, priority)
    private int[] minDistances;
    private List<int>[] shortestPath;

    public LevelGraph(int maxNumArenas)
    {
        // dijkstra's algo setup
        visited = new bool[maxNumArenas];
        shortestPath = new List<int>[maxNumArenas];
    }


    // Add arena (after generating location and adding to Grid)
    public bool AddArena(Vector2Int location)
    {
        // needs implementation
        return false;
    }

    // public function called by proceduralLevel to generate shortest path
    public void GenerateShortestPathTree()
    {
        // modify edge weights as desired prior to running shortest path algorithm

        // compute srcArena index (if not already done during arena generation) :
            // furthest distance from gridCenter (bossArena)

        // initialize dijkstra algorithm helper attributes
        for(int i = 0; i < numArenas; i++){
            int bossPriority = (-1) * generatedArenas[i].distToBoss;
            nodeQueue.Add((i, bossPriority));
            notVisited.Add(i);
            visited[i] = false;

            if(i == srcArena){
                minDistances[i] = 0;
                shortestPath[i] = null;
            }else{
                minDistances[i] = int.MaxValue;
                shortestPath[i].Add(srcArena);
            }
        }
        // source node should already be first in "priority queue" b/c it has largest dist to boss 

        Dijkstras();
    }

    // Dijkstras algorithm to compute shortest path tree
    // modified so it prioritizes next node according to their distance from center of grid (boss)
    public void Dijkstras()
    {
        (int index, int priority) currNode;

        while(!isQueueEmpty()){
            currNode = DequeueSmallest();

            // only perform actions if node hasn't been visited yet
            if(!visited[currNode.index]){
                visited[currNode.index] = true;

                // for each node adjacent to currently visiting one, calculate new path distance (through currNode)
                foreach((int index, int dist) adjNode in arenaAdjacencyList[currNode.index]){
                    int newDistance = minDistances[currNode.index] + adjNode.dist;
                
                    // if new distance is smaller, replace node minDistance and shortest path
                    if(newDistance < minDistances[adjNode.index]){
                        minDistances[adjNode.index] = newDistance;

                        shortestPath[adjNode.index] = shortestPath[currNode.index];
                        shortestPath[adjNode.index].Add(currNode.index);
                    }
                }
            }
        }
    }


    // makeshift priority queue helper functions (for now)
    public bool isQueueEmpty() => nodeQueue.Count > 0;

    public (int, int) DequeueSmallest()
    {
        var smallest = nodeQueue[0];
        for(int i = 1; i < numArenas; i++){
            if(nodeQueue[i].Item1 < smallest.Item1){
                smallest = nodeQueue[i];
            }
        }

        nodeQueue.Remove(smallest);
        return smallest;
    }
}