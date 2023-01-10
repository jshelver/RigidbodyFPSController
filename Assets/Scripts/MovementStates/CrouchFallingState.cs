using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helper;

public class CrouchFallingState : MovementBaseState
{
    public CrouchFallingState(MovementStateManager _stateManager, MovementStateMachine _stateMachine) : base(_stateManager, _stateMachine)
    {
        stateMachine = _stateMachine;
        stateManager = _stateManager;
    }

    public override void Enter()
    {
        base.Enter();

        stateManager.rb.drag = stateManager.airDrag;
        stateManager.targetHeight = stateManager.crouchHeight;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (Suspension.CheckIfGrounded(stateManager.feetTransform, -stateManager.transform.up, stateManager.suspensionRestDistance, stateManager.groundLayer))
        {
            // Switch to crouch state
            stateMachine.ChangeState(stateManager.crouchState);
            return;
        }

        if (InputManager.crouchInput)
        {
            // Switch to falling state
            stateMachine.ChangeState(stateManager.fallingState);
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

        stateManager.rb.AddForce(movementDir * (stateManager.crouchSpeed * stateManager.inAirModifier) * Time.deltaTime);
    }
}