using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{   
    // Not implemented for now. Player's stats will be stored here
    [Header("Movement")]
    public float moveSpeed; // Player's movement speed

    public float groundDrag; // Player's drag when on the ground

    public float jumpForce; // Player's jump force
    public float jumpCooldown; // Player's jump cooldown
    public float airMultiplier; // Player's movement speed multiplier when in the air
    bool canJump;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
