using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public Transform playerOrientation;
    [SerializeField] public LayerMask groundLayer;
    [HideInInspector] public Rigidbody rb;
    [SerializeField] public Transform feetTransform;

    [Header("State Machine / States")]
    public MovementStateMachine stateMachine;
    public IdleState idleState;
    public WalkState walkState;
    public RunState runState;
    public JumpState jumpState;
    public FallingState fallingState;

    [Header("Movement Settings")]
    [SerializeField] public float walkSpeed = 5f;
    [SerializeField] public float runSpeed = 9f;
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

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        stateMachine = new MovementStateMachine();
        idleState = new IdleState(this, stateMachine);
        walkState = new WalkState(this, stateMachine);
        runState = new RunState(this, stateMachine);
        jumpState = new JumpState(this, stateMachine);
        fallingState = new FallingState(this, stateMachine);

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

    public IEnumerator GroundedToFallingStateTimer()
    {
        // Wait for unknown seconds but check if grounded every frame
        for (float i = 0; i < timeInAirBeforeSwitchingToFallState; i += Time.deltaTime)
        {
            if (Helper.Suspension.CheckIfGrounded(feetTransform, -transform.up, suspensionRestDistance, groundLayer))
                yield break;
            yield return null;
        }
        stateMachine.ChangeState(fallingState);
    }

    public IEnumerator JumpToFallingStateTimer()
    {
        yield return new WaitForSeconds(0.1f);
        // wait for unknown seconds then switch to falling state
        for (float i = 0; i < jumpToFallingTimeDelay; i += Time.deltaTime)
        {
            if (Helper.Suspension.CheckIfGrounded(feetTransform, -transform.up, suspensionRestDistance, groundLayer))
            {
                if (InputManager.movementInput == Vector2.zero)
                {
                    // Switch to idle state
                    stateMachine.ChangeState(idleState);
                    yield break;
                }

                if (InputManager.runInput)
                {
                    // Switch to run state
                    stateMachine.ChangeState(runState);
                    yield break;
                }

                // Switch to walk state
                stateMachine.ChangeState(walkState);
                yield break;
            }
            yield return null;
        }
        stateMachine.ChangeState(fallingState);
    }
}
