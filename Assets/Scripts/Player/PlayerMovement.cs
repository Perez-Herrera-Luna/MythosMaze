using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// Based heavily on a movement controller tutorial by "Dave / Game Development"
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 10f; // Player's base walk speed
    
    private float moveSpeed; // Player's  current movement speed
    private float groundDrag = 5f; // Player's drag when on the ground

    private float desiredMoveSpeed; // Player's desired speed
    private float lastDesiredMoveSpeed; // Player's last desired speed
    private MovementState lastState; // Player's last movement state
    private bool keepMomemtum; // Used to keep player's momentum when changing movement state
    private float speedChangeFactor; // Used to change player's speed

    public bool isMoving = false;

    [Header("Jumping")]
    public float jumpForce = 7f; // Player's jump force
    public float jumpCooldown = 0.25f; // Player's jump cooldown
    public float airMultiplier = 0.4f; // Player's movement speed multiplier when in the air

    private bool canJump;

    // TODO: Remove crouching
    [Header("Crouching")]
    public float crouchSpeed = 3.5f; // Player's crouch speed
    public float crouchYScale = 0.5f; // Player's crouch height

    private float startYScale; // Player's starting height

    [Header("Ground Detection")]
    public float playerHeight = 2f; // Player's height. Used for length of raycast to detect ground
    public float additionalHeight = 0.2f; // Additional height added to the raycast to detect ground
    public LayerMask whatIsGround; // Layermask for ground detection

    [Header("Slope Handling")]
    public float maxSlopeAngle = 40f; // Maximum angle of slope the player can walk on

    private RaycastHit slopeHit; // Raycast to detect slope angle
    private bool exitingSlope; // Used to detect if the player is exiting a slope

    [Header("Sliding")]
    public float slideForce = 200f; // Force applied to the player when sliding

    private bool isSliding; // Used to track if the player is sliding
    private Vector3 slideDirection; // Direction of the slide

    [Header("Dashing")]
    public float dashSpeedModifier = 8f; // Player's dash speed modifier
    public float dashSpeed; // Player's dash speed

    public float dashForce = 18f;
    public float dashUpwardForce;

    public float dashDuration = 0.25f;
    public float dashCooldown = 1.5f;

    public float dashSpeedChangeFactor = 50f;

    private float dashCooldownTimer;
    private bool isDashing;

    public bool disableGravity = true;
    public bool resetVelocity = true;

    [Header("CameraEffects")]
    public float playerFov = 85f; // Player's default field of view
    public float changeDuration = 0.25f; // Duration of camera effect
    // public float dashFov = 95f; // Field of view when dashing
    public bool cameraTilt = true; // Should the camera tilt when moving?

    [Header("References")]
    public PlayerCamera cam; // Reference to the player's camera
    public Transform orientation; // Holds the player's orientation

    [Header("Debug")]
    public MovementState state; // Player's movement state. To be made private later
    public float currentSpeed; // Player's current speed. To be made private later
    public bool isGrounded; // Is the player on the ground?

    private float horizontalInput; // Player's horizontal input
    private float verticalInput; // Player's vertical input

    private Vector3 moveDirection; // Player's movement direction
    private Rigidbody rb; // Player's rigidbody

    [Header("Attacking")]
    public bool primaryAttack = false; //player attack flag
    public bool isAttacking = false;

    public int weaponSelected = 1;

    // [Header("Health UI")]

    // public PlayerHealthBar healthBar;

    private bool levelLoad = false;
    public bool LevelLoad
    {
        get => levelLoad;
        set => levelLoad = value;
    }
    private Vector3 initLoc;
    public Vector3 InitLoc
    {
        get => initLoc;
        set => initLoc = value;
    }

    public enum MovementState
    {
        walking,
        sliding,
        dashing,
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Freeze player's rotation

        playerFov = cam.GetDefaultFOV(); // Get player's default field of view
        ChangeFov(playerFov, 0f); // Set player's default field of view

        canJump = true; // Set jump state
        startYScale = transform.localScale.y; // Set player's starting height
        dashSpeed = walkSpeed + dashSpeedModifier; // Set player's dash speed

        // playerData.playerHealth = playerData.playerMaxHealth;
        // healthBar.SetMaxHealth(playerData.playerMaxHealth);
        // healthBar.SetHealth(playerData.playerHealth);
    }

    void Update()
    {
        CheckGrounded(); // Detect if the player is on the ground
        PlayerInput(); // Get player input
        SpeedControl(); // Limit player's speed
        StateHandler(); // Handle player's movement state
        MeasureSpeed(); // Measure player's speed
        CheckDrag(); // Apply drag based on player's state

        if (dashCooldownTimer > 0) // Decrement dash cooldown timer
        {
            dashCooldownTimer -= Time.deltaTime;
            UserInterfaceManager.inst.DisplayDashIndicator(dashCooldown, dashCooldownTimer);
        }

        //setting player health
        // healthBar.SetHealth(playerData.playerHealth);
    }

    void FixedUpdate()
    {
        MovePlayer(); // Move the player

        if (isSliding) // If the player is sliding, apply sliding movement
        {
            SlidingMovement();
        }

        if (levelLoad)
        {
            transform.position = initLoc;
            levelLoad = false;
        }
    }

    private void CheckGrounded()
    {
        if (isSliding) // If the player is sliding, use a shorter raycast for ground detection
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.25f + additionalHeight, whatIsGround);
        }
        else // Otherwise, use regular raycast for ground detection
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + additionalHeight, whatIsGround);
        }
    }


    private void PlayerInput()
    {
        ReadMoveInput(); // Read player's movement input
        
        if (InputManager.instance.JumpInput && isGrounded && canJump) // Jump if the player is on the ground and the jump key is pressed and the jump cooldown is over
        {
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown); // Reset jump cooldown
        }

        if (InputManager.instance.DashInput) // Dash if the dash key is pressed
        {
            Dash();
        }

        if (InputManager.instance.SlidePressed && !isSliding) // Slide if the slide key is pressed
        {
            StartSlide();
        }

        else if (InputManager.instance.SlideReleased && isSliding) // Stop sliding if the crouch key is released and the player is sliding
        {
            StopSlide();
        }

        if(horizontalInput < 0 && cameraTilt)
        {
            cam.applyTilt("left");
        }
    
        else if (horizontalInput > 0 && cameraTilt)
        {
            cam.applyTilt("right");
        }
        else if (horizontalInput == 0)
        {
            cam.applyTilt("reset");
        }

        //attacking
        if(InputManager.instance.AttackInput)
        {    
            //primaryAttack = true; 

            isAttacking = true;
            StartCoroutine(attackDelay());
            
        }
        
        //movement detection
        if(horizontalInput != 0 || verticalInput != 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        //weapon select   
        if(InputManager.instance.Weapon1Input)
        {
            Debug.Log("weapon 1 selected");
            weaponSelected = 1;
        }
        if(InputManager.instance.Weapon2Input)
        {
            Debug.Log("weapon 2 selected");
            weaponSelected = 2;
        }
        if(InputManager.instance.Weapon3Input)
        {
            Debug.Log("weapon 3 selected");
            weaponSelected = 3;
        }
    
    }

    IEnumerator attackDelay()
    {
        //Debug.Log("Delay start");
        yield return new WaitForSeconds(0.1f);
        //Debug.Log("Delay end");
        isAttacking = false;
    }

    private void ReadMoveInput()
    {
        horizontalInput = InputManager.instance.MovementInput.x;
        verticalInput = InputManager.instance.MovementInput.y;
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
    }

    private void CheckDrag()
    {
        if (isGrounded && !isDashing) // Apply drag when on the ground and not dashing
        {
            rb.drag = groundDrag;
        }
        else // Remove drag when in the air or dashing
        {
            rb.drag = 0f;
        }
    }

    // Preforms a jump
    private void Jump()
    {
        canJump = false;
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

    // Returns the player's input direction
    private Vector3 InputDirection()
    {
        return orientation.forward * verticalInput + orientation.right * horizontalInput;
    }

    private void StartSlide()
    {
        isSliding = true;

        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z); // Scale player's height down
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); // Apply force to keep player on ground as slide starts

        slideDirection = GetNormalizedInputDirectionVector();
    }

    private void StopSlide()
    {
        isSliding = false;

        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
    }

    private void SlidingMovement()
    {
        if (!OnSlope() || rb.velocity.y > -0.1f) // If the player is not on a slope or is moving upwards, preform regular slide
        {
            rb.AddForce(slideDirection * slideForce, ForceMode.Force);
        }
        else // If the player is on a slope, preform slide with slope movement
        {
            rb.AddForce(GetSlopeMoveDirection(slideDirection) * slideForce, ForceMode.Force);
        }
    }

    private void MeasureSpeed()
    {
        currentSpeed = rb.velocity.magnitude;
    }

    private Vector3 GetNormalizedInputDirectionVector()
    {
        float horizontal = InputManager.instance.MovementInput.x;
        float vertical = InputManager.instance.MovementInput.y;

        Vector3 direction = new Vector3();
        direction = orientation.forward * vertical + orientation.right * horizontal;

        if (vertical == 0 && horizontal == 0)
        {
            direction = orientation.forward;
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

        playerFov = cam.GetDefaultFOV();
        float dashFov = playerFov + 10f;
        ChangeFov(dashFov, changeDuration);

        Vector3 direction = GetNormalizedInputDirectionVector();
        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
        {
            rb.useGravity = false;
        }

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    private Vector3 delayedForceToApply; // Used to apply the dash force after a short delay. Hacky solution to a problem with the dash force not being applied properly

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