using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based heavily on a movement controller tutorial by "Dave / Game Development"
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed; // Player's movement speed

    public float groundDrag; // Player's drag when on the ground

    public float jumpForce; // Player's jump force
    public float jumpCooldown; // Player's jump cooldown
    public float airMultiplier; // Player's movement speed multiplier when in the air
    bool canJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space; // Keybind for jumping. Hardcoded to spacebar for now

    [Header("Ground Detection")]
    public float playerHeight; // Player's height. Used for length of raycast to detect ground
    public LayerMask whatIsGround; // Layermask for ground detection
    bool isGrounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        canJump = true;
    }

    void Update()
    {
        // Raycast to detect if the player is on the ground
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput(); // Get player input
        SpeedControl(); // Limit player's speed

        // Apply drag when on the ground
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0f;
        }
    }

    void FixedUpdate()
    {
        MovePlayer(); // Move the player
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && isGrounded && canJump)
        {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown); // Reset jump cooldown
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Apply force to the player. Variable airMultiplier is used to reduce the player's speed in the air
        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    // Limit the player's speed
    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    private void Jump()
    {
        // Reset player's velocity on Y axis
        // Ensure that the player always jumps the same height
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        canJump = true;
    }
}
