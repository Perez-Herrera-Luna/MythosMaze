using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public Vector2 CameraInput { get; private set; }
    public Vector2 MovementInput { get; private set; }
    public bool JumpInput { get; private set; }
    public bool DashInput { get; private set; }
    public bool SlidePressed { get; private set; }
    public bool SlideReleased { get; private set; }
    public bool AttackInput { get; private set; }
    public bool Weapon1Input { get; private set; }
    public bool Weapon2Input { get; private set; }
    public bool Weapon3Input { get; private set; }

    private PlayerInput playerInput;

    private InputAction cameraAction;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction slideAction;
    private InputAction dashAction;
    private InputAction attackAction;
    private InputAction weapon1;
    private InputAction weapon2;
    private InputAction weapon3;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        playerInput = GetComponent<PlayerInput>();

        SetupInputActions();
    }

    void Update()
    {
        UpdateInputs();
    }

    private void SetupInputActions()
    {
        cameraAction = playerInput.actions["Look"];
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        slideAction = playerInput.actions["Slide"];
        dashAction = playerInput.actions["Dash"];

        attackAction = playerInput.actions["Attack"];
        weapon1 = playerInput.actions["Weapon1"];
        weapon2 = playerInput.actions["Weapon2"];
        weapon3 = playerInput.actions["Weapon3"];
    }

    private void UpdateInputs()
    {
        CameraInput = cameraAction.ReadValue<Vector2>();
        MovementInput = moveAction.ReadValue<Vector2>();

        JumpInput = jumpAction.IsPressed();
        DashInput = dashAction.WasPressedThisFrame();

        SlidePressed = slideAction.WasPressedThisFrame();
        SlideReleased = slideAction.WasReleasedThisFrame();

        AttackInput = attackAction.triggered; // attackAction.IsPressed();
        Weapon1Input = weapon1.triggered; // weapon1.IsPressed();
        Weapon2Input = weapon2.triggered; // weapon2.IsPressed();
        Weapon3Input = weapon3.triggered; // weapon3.IsPressed();
    }

    public void DisableMovementInput()
    {
        moveAction.Disable();
        jumpAction.Disable();
        slideAction.Disable();
        dashAction.Disable();

        attackAction.Disable();
        weapon1.Disable();
        weapon2.Disable();
        weapon3.Disable();
    }

    public void EnableMovementInput()
    {
        moveAction.Enable();
        jumpAction.Enable();
        slideAction.Enable();
        dashAction.Enable();

        attackAction.Enable();
        weapon1.Enable();
        weapon2.Enable();
        weapon3.Enable();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        slideAction.Enable();
        dashAction.Enable();
        attackAction.Enable();
        weapon1.Enable();
        weapon2.Enable();
        weapon3.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        slideAction.Disable();
        dashAction.Disable();
        attackAction.Disable();
        weapon1.Disable();
        weapon2.Disable();
        weapon3.Disable();
    }
}
