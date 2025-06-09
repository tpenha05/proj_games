using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;

    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;
    public static bool RollWasPressed;
    public static bool DashWasPressed;
    public static bool AttackWasPressed;
    public static bool PowerWasPressed;
    public static bool RunRightHeld;
    public static bool RunLeftHeld;

    private InputAction _jumpAction;
    private InputAction _rollAction;
    private InputAction _dashAction;
    private InputAction _attackAction;
    private InputAction _powerAction;
    private InputAction _runRightAction;
    private InputAction _runLeftAction;

    private void Start()
    {
        PlayerInput.defaultControlScheme = "Touch";
    }


    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        _jumpAction     = PlayerInput.actions["Jump"];
        _rollAction     = PlayerInput.actions["Roll"];
        _dashAction     = PlayerInput.actions["Dash"];
        _attackAction   = PlayerInput.actions["Attack"];
        _powerAction    = PlayerInput.actions["Power"];
        _runRightAction = PlayerInput.actions["RunRight"];
        _runLeftAction  = PlayerInput.actions["RunLeft"];
    }

    private void Update()
    {
        JumpWasPressed   = _jumpAction.WasPressedThisFrame();
        JumpIsHeld       = _jumpAction.IsPressed();
        JumpWasReleased  = _jumpAction.WasReleasedThisFrame();
        RollWasPressed   = _rollAction.WasPressedThisFrame();
        DashWasPressed   = _dashAction.WasPressedThisFrame();
        AttackWasPressed = _attackAction.WasPressedThisFrame();
        PowerWasPressed  = _powerAction.WasPressedThisFrame();
        RunRightHeld     = _runRightAction.IsPressed();
        RunLeftHeld      = _runLeftAction.IsPressed();

    }
}
