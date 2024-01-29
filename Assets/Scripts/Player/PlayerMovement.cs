using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Based heavily on a movement controller tutorial by "Dave / Game Development"
public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed; // Player's movement speed

    [Header("Movement")]

    public float walkSpeed; // Player's walk speed
    public float sprintSpeed; // Player's run speed

    public float groundDrag; // Player's drag when on the ground

    [Header("Jumping")]

    public float jumpForce; // Player's jump force
    public float jumpCooldown; // Player's jump cooldown
    public float airMultiplier; // Player's movement speed multiplier when in the air
    bool canJump;

    [Header("Crouching")]
    public float crouchSpeed; // Player's crouch speed
    public float crouchYScale; // Player's crouch height
    private float startYScale; // Player's starting height

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space; // Keybind for jumping. Hardcoded to spacebar for now
    public KeyCode sprintKey = KeyCode.LeftShift; // Keybind for sprinting. Hardcoded to left shift for now
    public KeyCode crouchKey = KeyCode.LeftControl; // Keybind for crouching. Hardcoded to left control for now

    [Header("Ground Detection")]
    public float playerHeight; // Player's height. Used for length of raycast to detect ground
    public LayerMask whatIsGround; // Layermask for ground detection
    bool isGrounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle; // Maximum angle of slope the player can walk on
    private RaycastHit slopeHit; // Raycast to detect slope angle
    private bool exitingSlope; // Used to detect if the player is exiting a slope

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Rigidbody rb;

    public MovementState state;

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
        rb.freezeRotation = true;

        canJump = true;

        startYScale = transform.localScale.y;
    }

    void Update()
    {
        // Raycast to detect if the player is on the ground
        if (state != MovementState.crouching)
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);
        }
        else
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.25f + 0.3f, whatIsGround);
        }

        // Debug
        // Debug.Log(isGrounded);

        MyInput(); // Get player input
        SpeedControl(); // Limit player's speed
        StateHandler(); // Handle player's movement state

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

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if (isGrounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (isGrounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        // Apply force to the player. Variable airMultiplier is used to reduce the player's speed in the air
        else if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        // turn gravity off while on a slope
        rb.useGravity = !OnSlope();
    }

    // Limit the player's speed
    private void SpeedControl()
    {
        // limiting speed on a slope
        if (OnSlope() && !exitingSlope)
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

        // Reset player's velocity on Y axis
        // Ensure that the player always jumps the same height
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        canJump = true;

        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
