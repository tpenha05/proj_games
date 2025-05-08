using UnityEngine;

[CreateAssetMenu(menuName = "Player/Movement Stats")]
public class PlayerMovementStats : ScriptableObject
{
    [Header("Horizontal")]
    [Range(1f, 20f)]  public float WalkSpeed     = 6f;
    [Range(1f, 30f)]  public float RunSpeed      = 10f;
    [Range(0f, 100f)] public float Acceleration  = 40f;
    [Range(0f, 100f)] public float Deceleration  = 50f;

    [Header("Jump")]
    [Range(1f, 25f)]  public float JumpForce         = 14f;
    [Range(0f, 1f)]   public float JumpCutMultiplier = .5f;
    [Range(5f, 50f)]  public float MaxFallSpeed      = 25f;

    [Header("Coyote & Buffer (seg)")]
    [Range(0f, .5f)]  public float CoyoteTime     = .1f;
    [Range(0f, .5f)]  public float JumpBufferTime = .1f;

    [Header("Ground Check")]
    public Vector2   GroundCheckSize = new(0.4f, 0.1f);
    public LayerMask GroundLayer;

    [Header("Debug")]
    public bool DebugShowGroundBox;
}
