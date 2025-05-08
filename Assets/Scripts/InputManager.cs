using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;

    public static Vector2 Movement;
    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;
    public static bool RunIsHeld;
    public static bool RollWasPressed;
    public static bool DashWasPressed;
    public static bool DropWasPressed;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _rollAction;
    private InputAction _dashAction;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _runAction = PlayerInput.actions["Run"];
        _rollAction = PlayerInput.actions["Roll"];
        _dashAction = PlayerInput.actions["Dash"];

    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();

        JumpWasPressed = _jumpAction.WasPressedThisFrame();
        JumpIsHeld = _jumpAction.IsPressed();
        JumpWasReleased = _jumpAction.WasReleasedThisFrame();
        RollWasPressed = _rollAction.WasPressedThisFrame();
        DashWasPressed = _dashAction.WasPressedThisFrame();
        DropWasPressed = _runAction.WasPressedThisFrame();

        RunIsHeld = _runAction.IsPressed();
    }
}
