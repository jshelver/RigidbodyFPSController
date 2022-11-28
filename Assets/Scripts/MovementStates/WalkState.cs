using UnityEngine;
using Helper;

public class WalkState : MovementBaseState
{
    public WalkState(MovementStateManager _stateManager, MovementStateMachine _stateMachine) : base(_stateManager, _stateMachine)
    {
        stateMachine = _stateMachine;
        stateManager = _stateManager;
    }

    public override void Enter()
    {
        base.Enter();
        stateManager.rb.drag = stateManager.groundDrag;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (InputManager.movementInput == Vector2.zero)
        {
            // Switch to idle state
            stateMachine.ChangeState(stateManager.idleState);
            return;
        }
        if (InputManager.runInput)
        {
            // Switch to run state
            stateMachine.ChangeState(stateManager.runState);
            return;
        }

        if (InputManager.jumpInput)
        {
            // Switch to jump state
            stateMachine.ChangeState(stateManager.jumpState);
            return;
        }

        if (!Suspension.CheckIfGrounded(stateManager.feetTransform, -stateManager.transform.up, stateManager.suspensionRestDistance, stateManager.groundLayer))
        {
            stateManager.StartCoroutine(stateManager.GroundedToFallingStateTimer());
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 movementDir = stateManager.playerOrientation.forward * InputManager.movementInput.y + stateManager.playerOrientation.right * InputManager.movementInput.x;

        if (Physics.Raycast(stateManager.rb.transform.position, Vector3.down, out RaycastHit hit, 2f, stateManager.groundLayer))
        {
            movementDir = ConvertMovementDirectionToSlopeAngle(movementDir, hit);
        }

        stateManager.rb.AddForce(movementDir * stateManager.walkSpeed * Time.deltaTime);
    }

    private Vector3 ConvertMovementDirectionToSlopeAngle(Vector3 movementDir, RaycastHit hit)
    {
        // Get the angle between up and slope's normal
        float groundSlopeAngle = Vector3.Angle(hit.normal, Vector3.up);
        if (groundSlopeAngle != 0f)
        {
            // Basically gives the amount of rotation to get from up to slopeNormal
            Quaternion slopeAngleRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            // Multiply movementDir by this Quaternion so now movementDir is perpendicular to the slope's normal, instead of up
            movementDir = slopeAngleRotation * movementDir;
        }
        Debug.DrawRay(stateManager.rb.transform.position, movementDir, Color.red, 0.5f);
        return movementDir;
    }
}
