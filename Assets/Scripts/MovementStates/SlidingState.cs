using System.Collections;
using Helper;
using UnityEngine;

public class SlidingState : MovementBaseState
{
    public SlidingState(MovementStateManager _stateManager, MovementStateMachine _stateMachine) : base(_stateManager, _stateMachine)
    {
        stateMachine = _stateMachine;
        stateManager = _stateManager;
    }

    public override void Enter()
    {
        base.Enter();
        stateManager.rb.drag = 0f;
        stateManager.targetHeight = stateManager.crouchHeight;

        stateManager.rb.AddForce(stateManager.rb.velocity.normalized * stateManager.initialSlideSpeedBurst, ForceMode.VelocityChange);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (InputManager.crouchInput)
        {
            if (InputManager.movementInput == Vector2.zero)
            {
                // Switch to idle state
                stateMachine.ChangeState(stateManager.idleState);
                return;
            }

            if (InputManager.runInput && InputManager.movementInput.y > 0f)
            {
                // Switch to run state
                stateMachine.ChangeState(stateManager.runState);
                return;
            }

            // Switch to walk state
            stateMachine.ChangeState(stateManager.walkState);
            return;
        }

        Vector3 flatVelocity = new Vector3(stateManager.rb.velocity.x, 0f, stateManager.rb.velocity.z);
        if (flatVelocity.magnitude < 2f)
        {
            if (InputManager.movementInput == Vector2.zero)
            {
                // Switch to idle state
                stateMachine.ChangeState(stateManager.idleState);
                return;
            }

            if (InputManager.runInput && InputManager.movementInput.y > 0f)
            {
                // Switch to run state
                stateMachine.ChangeState(stateManager.runState);
                return;
            }

            // Switch to walk state
            stateMachine.ChangeState(stateManager.walkState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // To do: Add sliding logic
        if (Physics.Raycast(stateManager.rb.transform.position, Vector3.down, out RaycastHit hit, 2f, stateManager.groundLayer))
        {
            // Get the angle between up and slope's normal
            float groundSlopeAngle = Vector3.Angle(hit.normal, Vector3.up);

            if (groundSlopeAngle > 0.01f || groundSlopeAngle < -0.01f)
            {
                // On slope
                HandleSlideVelocityOnSlope(hit);
            }
            else
            {
                // On flat surface
                HandleSlideVelocityOnFlatSurface();
            }

        }
    }

    public override void Exit()
    {
        base.Exit();

        stateManager.targetHeight = stateManager.originalHeight;
    }

    private void HandleSlideVelocityOnSlope(RaycastHit _hit)
    {
        // Get vector that points down the slope (perpendicular to the slope's normal)
        Vector3 perpendicularToSlopeDirection = Vector3.Cross(Vector3.up, _hit.normal);
        Vector3 downwardSlopeDirection = Vector3.Cross(perpendicularToSlopeDirection, _hit.normal);

        // Add downward force to simulate sliding down the slope
        stateManager.rb.AddForce(downwardSlopeDirection * stateManager.downhillSlideAcceleration, ForceMode.Acceleration);
    }

    private void HandleSlideVelocityOnFlatSurface()
    {
        stateManager.rb.AddForce(-stateManager.rb.velocity.normalized * stateManager.slidingFriction, ForceMode.Acceleration);
    }
}
