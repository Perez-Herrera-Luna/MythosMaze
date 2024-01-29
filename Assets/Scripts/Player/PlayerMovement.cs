using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Based heavily on a movement controller tutorial by "Dave / Game Development"
public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed; // Player's movement speed

    [Header("Movement")]

    public float walkSpeed = 7f; // Player's walk speed
    public float sprintSpeed = 10f; // Player's run speed
    public float groundDrag = 5f; // Player's drag when on the ground

    [Header("Jumping")]

    public float jumpForce = 7f; // Player's jump force
    public float jumpCooldown = 0.25f; // Player's jump cooldown
    public float airMultiplier = 0.4f; // Player's movement speed multiplier when in the air
    bool canJump;

    [Header("Crouching")]
    public float crouchSpeed = 3.5f; // Player's crouch speed
    public float crouchYScale = 0.5f; // Player's crouch height
    private float startYScale; // Player's starting height

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space; // Keybind for jumping. Hardcoded to spacebar for now
    public KeyCode sprintKey = KeyCode.LeftShift; // Keybind for sprinting. Hardcoded to left shift for now
    public KeyCode crouchKey = KeyCode.LeftControl; // Keybind for crouching. Hardcoded to left control for now

    [Header("Ground Detection")]
    public float playerHeight = 2f; // Player's height. Used for length of raycast to detect ground
    public LayerMask whatIsGround; // Layermask for ground detection
    bool isGrounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle = 40f; // Maximum angle of slope the player can walk on
    private RaycastHit slopeHit; // Raycast to detect slope angle
    private bool exitingSlope; // Used to detect if the player is exiting a slope

    [Header("Other")]
    public Transform orientation;
    public MovementState state;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Freeze player's rotation
        canJump = true;
        startYScale = transform.localScale.y; // Set player's starting height
    }

    void Update()
    {
        if (state != MovementState.crouching) // Raycast to detect if the player is on the ground
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);
        }
        else // If the player is crouching, use a shorter raycast
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.25f + 0.3f, whatIsGround);
        }

        MyInput(); // Get player input
        SpeedControl(); // Limit player's speed
        StateHandler(); // Handle player's movement state

        if (isGrounded) // Apply drag when on the ground
        {
            rb.drag = groundDrag;
        }
        else // Remove drag when in the air
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

        if (Input.GetKey(jumpKey) && isGrounded && canJump) // Jump if the player is on the ground and the jump key is pressed and the jump cooldown is over
        {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown); // Reset jump cooldown
        }

        if (Input.GetKeyDown(crouchKey)) // Crouch if the crouch key is pressed
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchKey)) // Uncrouch if the crouch key is released
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        if (Input.GetKey(crouchKey)) // Crouching
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if (isGrounded && Input.GetKey(sprintKey)) // Sprinting
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (isGrounded) // Walking
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else // In the air
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope) // Move the player on a slope
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force); // Apply extra force to keep the player on the ground while on slopes
            }
        }
        else if (isGrounded) // Move the player on flat ground
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!isGrounded) // Move the player in the air. Air movement is slower than ground movement
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        rb.useGravity = !OnSlope(); // Turn gravity off while on a slope
    }

    // Limit the player's speed
    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope) // limiting speed on a slope
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        else // limiting speed on flat ground or in air
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVelocity.magnitude > moveSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset player's velocity on Y axis. Ensures that the player always jumps the same height

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        canJump = true;
        exitingSlope = false;
    }

    // Detect if the player is on a slope
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    // Returns a normalized vector of the player's movement direction on a slope
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
