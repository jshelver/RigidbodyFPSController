using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovementStateManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public Transform playerOrientation;
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] public LayerMask wallRunnableLayers;
    [HideInInspector] public Rigidbody rb;
    [SerializeField] public Transform feetTransform;
    [SerializeField] public Transform cameraTransform;
    public static Action<float> OnCameraZAngleChanged;

    [Header("State Machine / States")]
    public MovementStateMachine stateMachine;
    public IdleState idleState;
    public WalkState walkState;
    public RunState runState;
    public JumpState jumpState;
    public FallingState fallingState;
    public WallRunState wallRunState;
    public WallJumpState wallJumpState;

    [Header("Movement Settings")]
    [SerializeField] public float walkSpeed = 5f;
    [SerializeField] public float runSpeed = 9f;
    [SerializeField] public float inAirModifier = 0.15f; // Based off of the walk speed
    [SerializeField] public float groundDrag = 5f;
    [SerializeField] public float airDrag = 0.2f;

    [Header("Suspension Settings")]
    [SerializeField] public float springStrength = 5000f;
    [SerializeField] public float springDamper = 40f;
    [SerializeField] public float suspensionRestDistance = .8f;

    [Header("Jump/Falling Settings")]
    [SerializeField] public float jumpForce = 20f;
    [SerializeField] public float timeInAirBeforeSwitchingToFallState = .2f;
    [SerializeField] public float jumpToFallingTimeDelay = 1f;

    [Header("Wall Running/Jumping Settings")]
    [SerializeField] public float wallRunSpeed = 5f;
    [SerializeField] public float maxWallRunSpeed = 100f;
    [SerializeField] public float maxDistanceFromWall = 1.5f;
    [SerializeField] public float minimumHeightToWallRun = 0.5f;
    [SerializeField] public float cameraZAngle = 20f;
    [Space(12)]
    [SerializeField] public float wallJumpUpForce = 20f;
    [SerializeField] public float wallJumpPushOffForce = 20f;
    [SerializeField] public float wallJumpToWallRunCooldown = 0.15f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        stateMachine = new MovementStateMachine();
        idleState = new IdleState(this, stateMachine);
        walkState = new WalkState(this, stateMachine);
        runState = new RunState(this, stateMachine);
        jumpState = new JumpState(this, stateMachine);
        fallingState = new FallingState(this, stateMachine);
        wallRunState = new WallRunState(this, stateMachine);
        wallJumpState = new WallJumpState(this, stateMachine);

        stateMachine.Initialize(idleState);
    }

    void Update()
    {
        stateMachine.currentState.LogicUpdate();
    }

    void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }
}
