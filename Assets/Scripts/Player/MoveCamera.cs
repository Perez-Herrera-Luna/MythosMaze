using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is used to move the camera to the player's position
// Supposedly this is a better way to do it than parenting the camera to the player
// Based heavily on a movement controller tutorial by "Dave / Game Development"
public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;

    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
