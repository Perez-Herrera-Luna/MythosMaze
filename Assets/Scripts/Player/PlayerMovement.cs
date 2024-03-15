using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// TODO: Add camera rotation when sliding
// TODO: Interpolate between sliding and crouching
// TODO: Handle jumping out of a slide or crouch properly
// TODO: Add a wall jump
// TODO: Allow dashing during start of jump
// TODO: Prevent entering crouch while in air
// TODO: Handle dashing off or into a slope
// TODO: Add double/triple jump
// TODO: Give player a limited number of dashes
// TODO: Add a dash meter
// TODO: Add a slam in the air

// Based heavily on a movement controller tutorial by "Dave / Game Development"
public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed; // Player's movement speed

    [Header("Movement")]

    public float walkSpeed = 10f; // Player's walk speed
    public float dashSpeed = 18f; // Player's dash speed
    public float groundDrag = 5f; // Player's drag when on the ground
    private float maxYSpeed;

    private float desiredMoveSpeed; // Player's desired speed
    private float lastDesiredMoveSpeed; // Player's last desired speed
    private MovementState lastState; // Player's last movement state
    private bool keepMomemtum; // Used to keep player's momentum when changing movement state
    private float speedChangeFactor; // Used to change player's speed

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
    public KeyCode crouchKey = KeyCode.LeftControl; // Keybind for crouching. Hardcoded to left control for now
    public KeyCode dashKey = KeyCode.LeftShift; // Keybind for dashing. Hardcoded to left shift for now

    [Header("Ground Detection")]
    public float playerHeight = 2f; // Player's height. Used for length of raycast to detect ground
    public LayerMask whatIsGround; // Layermask for ground detection

    [Header("Slope Handling")]
    public float maxSlopeAngle = 40f; // Maximum angle of slope the player can walk on
    private RaycastHit slopeHit; // Raycast to detect slope angle
    private bool exitingSlope; // Used to detect if the player is exiting a slope

    [Header("Sliding")]
    public float maxSlideTime = 0.75f; // Maximum time the player can slide
    public float slideForce = 200f; // Force applied to the player when sliding
    private float slideTimer; // Timer for sliding
    private bool isSliding; // Used to track if the player is sliding

    [Header("Dashing")]
    public float dashForce = 18f;
    public float dashUpwardForce;
    public float dashDuration = 0.25f;
    public float dashCooldown = 1.5f;
    public float dashSpeedChangeFactor = 50f;
    public float maxDashYSpeed = 18f;
    private float dashCooldownTimer;
    private bool isDashing;

    public bool useCameraForward = false;
    public bool allowAllDirections = true;
    public bool disableGravity = true;
    public bool resetVelocity = true;

    [Header("CameraEffects")]
    public float playerFov = 85f; // Player's default field of view
    public float changeDuration = 0.25f; // Duration of camera effect
    public float dashFov = 95f; // Field of view when dashing

    [Header("References")]
    public PlayerCamera cam; // Reference to the player's camera
    public Transform orientation; // Holds the player's orientation

    [Header("Debug")]
    public MovementState state; // Player's movement state. To be made private later
    public float currentSpeed; // Player's current speed. To be made private later
    public bool isGrounded; // Is the player on the ground?

    [Header("Attacking")]
    public KeyCode attackKey = KeyCode.Mouse0; // Keybind for primary attack. Hardcoded to spacebar for now
    public bool primaryAttack = false; //player attack flag
    public bool attackEnabled = true;

    float horizontalInput; // Player's horizontal input
    float verticalInput; // Player's vertical input

    Vector3 moveDirection; // Player's movement direction
    Rigidbody rb; // Player's rigidbody

    public enum MovementState
    {
        walking,
        crouching,
        sliding,
        dashing,
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Freeze player's rotation
        ChangeFov(playerFov, 0f); // Set player's default field of view
        canJump = true;
        startYScale = transform.localScale.y; // Set player's starting height
    }

    void Update()
    {
        if (state == MovementState.crouching || state == MovementState.sliding) // If the player is crouching or sliding, use a shorter raycast for ground detection
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.25f + 0.2f, whatIsGround);
        }
        else // Otherwise, use regular raycast for ground detection
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        }

        MyInput(); // Get player input
        SpeedControl(); // Limit player's speed
        StateHandler(); // Handle player's movement state
        MeasureSpeed(); // Measure player's speed

        if (isGrounded && !isDashing) // Apply drag when on the ground and not dashing
        {
            rb.drag = groundDrag;
        }
        else // Remove drag when in the air or dashing
        {
            rb.drag = 0f;
        }

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        MovePlayer(); // Move the player

        if (isSliding) // If the player is sliding, apply sliding movement
        {
            SlidingMovement();
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(dashKey))
        {
            Dash();
        }

        if (Input.GetKey(jumpKey) && isGrounded && canJump) // Jump if the player is on the ground and the jump key is pressed and the jump cooldown is over
        {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown); // Reset jump cooldown
        }

        if (Input.GetKeyDown(crouchKey) && IsMoving()) // Slide if the player is moving and the crouch key is pressed
        {
            StartSlide();
        }

        if (Input.GetKeyUp(crouchKey) && isSliding) // Stop sliding if the crouch key is released and the player is sliding
        {
            StopSlide();
        }

        if (Input.GetKeyDown(crouchKey) && !IsMoving()) // Crouch if the crouch key is pressed and the player is not moving
        {
            StartCrouch();
        }

        if (Input.GetKeyUp(crouchKey) && !isSliding) // Uncrouch if the crouch key is released and the player is not sliding
        {
            StopCrouch();
        }

        if(horizontalInput == -1)
        {
            cam.applyTilt("left");
        }
    
        else if (horizontalInput == 1)
        {
            cam.applyTilt("right");
        }
        else if (horizontalInput == 0)
        {
            cam.applyTilt("reset");
        }

        //attacking
        if(Input.GetKey(attackKey) && attackEnabled)
        {
            //Debug.Log("attack key pressed");
            primaryAttack = true;
            attackEnabled = false;
            StartCoroutine(attackCoolDown());
        }
        else if(Input.GetKeyUp(attackKey))
        {
            primaryAttack = false;
        }
        
    }

    IEnumerator attackCoolDown()
    {
        yield return new WaitForSeconds(0.25f);
        attackEnabled = true;
    }

    private void StateHandler()
    {
        if (isDashing) // Dashing
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
        }
        else if (isSliding) // Sliding
        {
            state = MovementState.sliding;
            desiredMoveSpeed = walkSpeed;
        }
        else if (Input.GetKey(crouchKey) && !isSliding) // Crouching
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        else if (isGrounded) // Walking
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        if (lastState == MovementState.dashing)
        {
            keepMomemtum = true;
        }

        if (desiredMoveSpeed != lastDesiredMoveSpeed)
        {
            if (keepMomemtum)
            {
                StopAllCoroutines();
                StartCoroutine(LerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }

    // Move the player based on their movement state
    private void MovePlayer()
    {
        if (state == MovementState.dashing)
        {
            return;
        }

        moveDirection = InputDirection(); // Get player's movement direction

        if (OnSlope() && !exitingSlope) // Move the player on a slope
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

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
        if (OnSlope() && !exitingSlope) // Limiting speed on a slope
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        else // Limiting speed on flat ground or in air
        {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVelocity.magnitude > moveSpeed)
            {
                Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
        }

        if (maxYSpeed != 0 && rb.velocity.y > maxYSpeed) // Limiting vertical speed
        {
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
        }
    }

    // Preforms a jump
    private void Jump()
    {
        exitingSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset player's velocity on Y axis. Ensures that the player always jumps the same height

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    // Ensures that the player can jump again after the jump cooldown
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
    private Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    // Returns true if the player is inputting movement
    private bool IsMoving()
    {
        return horizontalInput != 0 || verticalInput != 0;
    }

    // Returns the player's input direction
    private Vector3 InputDirection()
    {
        return orientation.forward * verticalInput + orientation.right * horizontalInput;
    }

    private void StartSlide()
    {
        isSliding = true;

        StartCrouch();

        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        if (!OnSlope() || rb.velocity.y > -0.1f) // If the player is not on a slope or is moving upwards, preform regular slide
        {
            rb.AddForce(InputDirection().normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }
        else
        {
            rb.AddForce(GetSlopeMoveDirection(InputDirection()) * slideForce, ForceMode.Force);
        }

        if (slideTimer <= 0)
        {
            isSliding = false;
        }
    }

    private void StopSlide()
    {
        isSliding = false;

        StopCrouch();
    }

    private void StartCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }

    private void StopCrouch()
    {
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
    }


    private void MeasureSpeed()
    {
        currentSpeed = rb.velocity.magnitude;
    }

    private Vector3 GetDashDirection(Transform forwardT)
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        if (allowAllDirections)
        {
            direction = forwardT.forward * vertical + forwardT.right * horizontal;
        }
        else
        {
            direction = forwardT.forward;
        }

        if (vertical == 0 && horizontal == 0)
        {
            direction = forwardT.forward;
        }

        return direction.normalized;
    }

    private void Dash()
    {
        if (dashCooldownTimer > 0)
        {
            return;
        }
        else
        {
            dashCooldownTimer = dashCooldown;
        }

        isDashing = true;
        maxYSpeed = maxDashYSpeed;

        ChangeFov(dashFov, changeDuration);

        Transform forwardT;

        if (useCameraForward)
        {
            forwardT = cam.transform;
        }
        else
        {
            forwardT = orientation;
        }

        Vector3 direction = GetDashDirection(forwardT);

        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
        {
            rb.useGravity = false;
        }

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 delayedForceToApply; // Used to apply the dash force after a short delay. Hacky solution to a problem with the dash force not being applied properly. Replace with coroutine later

    private void DelayedDashForce()
    {
        if (resetVelocity)
        {
            rb.velocity = Vector3.zero;
        }

        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        isDashing = false;
        maxYSpeed = 0;

        ChangeFov(playerFov, changeDuration);

        if (disableGravity)
        {
            rb.useGravity = true;
        }
    }

    private IEnumerator LerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startSpeed = moveSpeed;
        float changeFactor = speedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startSpeed, desiredMoveSpeed, time / difference);
            time += Time.deltaTime * changeFactor;
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomemtum = false;
    }

    private void ChangeFov(float targetFov, float duration)
    {
        cam.SmoothFovChange(targetFov, duration);
    }
}