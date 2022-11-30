using UnityEngine;
using Helper;
using System.Collections;

public class RunState : MovementBaseState
{
    public RunState(MovementStateManager _stateManager, MovementStateMachine _stateMachine) : base(_stateManager, _stateMachine)
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
        if (!InputManager.runInput)
        {
            // Switch to walk state
            stateMachine.ChangeState(stateManager.walkState);
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
            stateManager.StartCoroutine(GroundedToFallingStateTimer());
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

        stateManager.rb.AddForce(movementDir.normalized * stateManager.runSpeed * Time.deltaTime);
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

    private IEnumerator GroundedToFallingStateTimer()
    {
        // Wait for unknown seconds but check if grounded every frame
        for (float i = 0; i < stateManager.timeInAirBeforeSwitchingToFallState; i += Time.deltaTime)
        {
            if (Helper.Suspension.CheckIfGrounded(stateManager.feetTransform, -stateManager.transform.up, stateManager.suspensionRestDistance, stateManager.groundLayer))
                yield break;
            yield return null;
        }
        stateMachine.ChangeState(stateManager.fallingState);
    }
}
