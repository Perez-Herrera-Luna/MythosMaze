using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePriorityQueue
{
    private Dictionary<int, float> queue;

    public NodePriorityQueue()
    {
        queue = new Dictionary<int, float>();
    }

    public void AddNode(int i, float d)
    {
        if(!queue.TryAdd(i, d))
            Debug.Log("Error: Tried adding existing node to priority queue");
    }

    public bool IsEmpty(){
        if(queue.Count > 0)
            return false;
        else
            return true;
    }

    // Dequeue smallest from queue (according to priority float)
    public (int, float) DequeueSmallest()
    {
        int index = SmallestIndex();
        float value = queue[index];

        if(!queue.Remove(index))
            Debug.Log("Error dequeue smallest node value");

        return (index, value);
    }

    public int SmallestIndex()
    {
        if(queue.Count == 0)
            return -1;      // empty node queue


        int i = 0;
        int smallestIndex = -1;
        float smallestVal = float.MaxValue;
        foreach( KeyValuePair<int, float> kvp in queue){
            if(i == 0){
                smallestIndex = kvp.Key;
                smallestVal = kvp.Value;
            }else{
                if(kvp.Value < smallestVal){
                    smallestIndex = kvp.Key;
                    smallestVal = kvp.Value;
                }
            }
            i++;
        }

        return smallestIndex;
    }

    // returns distance corresponding to node with index i
    // returns -1 if index is not in queue
    public float GetDistance(int i)
    {
        float value;

        if(queue.TryGetValue(i, out value))
            return value;
        else
            return -1;
    }

    // update distance corresponding to node with index i
    public void UpdateDistance(int i, float d)
    {
        queue[i] = d;
    }

    public void PrintPriorityQueue()
    {
        for(int i = 0; i < queue.Count; i++){
            Debug.Log("index: " + i + ", value: " + GetDistance(i));
        }
    }
}