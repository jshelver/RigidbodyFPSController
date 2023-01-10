using System.Collections;
using Helper;
using UnityEngine;

public class CrouchState : MovementBaseState
{
    bool readyToSprint;

    public CrouchState(MovementStateManager _stateManager, MovementStateMachine _stateMachine) : base(_stateManager, _stateMachine)
    {
        stateMachine = _stateMachine;
        stateManager = _stateManager;
    }

    public override void Enter()
    {
        base.Enter();

        stateManager.rb.drag = stateManager.groundDrag;
        stateManager.targetHeight = stateManager.crouchHeight;

        readyToSprint = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (InputManager.jumpInput || InputManager.crouchInput)
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

        // If the run button has been released since entering crouch state, then we can sprint again
        if (!InputManager.runInput) readyToSprint = true;

        // Allow player to run out of crouch state
        if (InputManager.runInput && InputManager.movementInput.y > 0f && readyToSprint)
        {
            // Switch to run state
            stateMachine.ChangeState(stateManager.runState);
            return;
        }

        // If the player is not grounded, then start timer to switch to falling state
        if (!Suspension.CheckIfGrounded(stateManager.feetTransform, -stateManager.transform.up, stateManager.suspensionRestDistance, stateManager.groundLayer))
        {
            stateManager.StartCoroutine(GroundedToCrouchFallingStateTimer());
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        HandleMovement();
    }

    public override void Exit()
    {
        base.Exit();

        stateManager.targetHeight = stateManager.originalHeight;
    }

    private void HandleMovement()
    {
        Vector3 movementDir = stateManager.playerOrientation.forward * InputManager.movementInput.y + stateManager.playerOrientation.right * InputManager.movementInput.x;

        if (Physics.Raycast(stateManager.rb.transform.position, Vector3.down, out RaycastHit hit, 2f, stateManager.groundLayer))
        {
            movementDir = ConvertMovementDirectionToSlopeAngle(movementDir, hit);
        }

        stateManager.rb.AddForce(movementDir.normalized * stateManager.crouchSpeed * Time.deltaTime);
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
        return movementDir;
    }

    private IEnumerator GroundedToCrouchFallingStateTimer()
    {
        // Wait for unknown seconds but check if grounded every frame
        for (float i = 0; i < stateManager.timeInAirBeforeSwitchingToFallState; i += Time.deltaTime)
        {
            if (Helper.Suspension.CheckIfGrounded(stateManager.feetTransform, -stateManager.transform.up, stateManager.suspensionRestDistance, stateManager.groundLayer))
                yield break;
            yield return null;
        }
        stateMachine.ChangeState(stateManager.crouchFallingState);
    }
}
