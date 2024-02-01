using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode
{
    private char nodeValue = 'E'; // values can be: E, X, A, D, P, T, I
    private int numConnections = 0;
    private List<Vector2Int> currConnections;   // stores Vector2Int.up, .down, .left, and .right
    public List<Vector2Int> currCorners;

    public GridNode(char val) => nodeValue = val;

    public char NodeValue
    { 
        get => nodeValue;
        set => nodeValue = value;
    }
    public int NumConnections => numConnections;

    // helper function called in process of adding new path connection
    // blocks corner if not already in use
    public void BlockCorner()
    {
        if(nodeValue == 'E')
            nodeValue = 'X';
    }

    // function which adds path connection in given direction to currNode
    // returns true if successful, false if unable to add connection
    public bool AddConnection(Vector2Int newConnection){
        if(nodeValue == 'E'){
            numConnections++;
            currConnections.Add(newConnection);
            return true;
        }else if(nodeValue == 'P'){
            // if path is only connected on one end currently
            if(numConnections == 1){
                // check if new connection is parallel or perpendicular to currConnection
                float dotProduct = Vector2.Dot(currConnections[0], newConnection);

                if(dotProduct == 1){
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
                }else if(dotProduct == -1){
                    // new connection is the same as existing connection
                    Debug.Log("Error: trying to add duplicate path connection");
                    return false; 
                }else{
                    // invalid dot product result - smtg is seriously wrong (vectors probably not normalized)
                    Debug.Log("Error: invalid dotProduct value when adding path connection");
                    return false; 
                }
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
        }else if(nodeValue == 'T'){
            // adding connection to turn (changes to junction)
            if(numConnections == 2){
                nodeValue = 'J';
                numConnections++;
                currConnections.Add(newConnection);

                // add appropriate corner to list
                if(Vector2.Dot(currConnections[0], newConnection) == 0){
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
        }else if(nodeValue == 'J'){
            // adding connection to 3-way joint
            if(numConnections == 3){
                numConnections++;
                currConnections.Add(newConnection);

                // add appropriate corners to list
                // find which connection is parallel to newConnection, use other 2 connections to calculate corner
                if(Vector2.Dot(currConnections[0], newConnection) == 1){
                    currCorners.Add(currConnections[1] + newConnection);
                    currCorners.Add(currConnections[2] + newConnection);
                }else if(Vector2.Dot(currConnections[1], newConnection) == 1){
                    currCorners.Add(currConnections[0] + newConnection);
                    currCorners.Add(currConnections[2] + newConnection);
                }else if(Vector2.Dot(currConnections[2], newConnection) == 1){
                    currCorners.Add(currConnections[0] + newConnection);
                    currCorners.Add(currConnections[1] + newConnection);
                }else{
                    Debug.Log("Error: invalid dotproduct value");
                    return false;
                }
                return true;
            }else{
                Debug.Log("Error: trying to connect to junction with invalid numConnections");
                return false;
            }
        }else{
            Debug.Log("Error: trying to connect to invalid nodeType");
            return false;   
        }
    }
}
