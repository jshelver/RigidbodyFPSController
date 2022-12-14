using UnityEngine;
using Helper;

public class FallingState : MovementBaseState
{
    public FallingState(MovementStateManager _stateManager, MovementStateMachine _stateMachine) : base(_stateManager, _stateMachine)
    {
        stateMachine = _stateMachine;
        stateManager = _stateManager;
    }

    public override void Enter()
    {
        base.Enter();
        stateManager.rb.drag = stateManager.airDrag;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (Suspension.CheckIfGrounded(stateManager.feetTransform, -stateManager.transform.up, stateManager.suspensionRestDistance, stateManager.groundLayer))
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

        stateManager.rb.AddForce(movementDir * (stateManager.walkSpeed * stateManager.inAirModifier) * Time.deltaTime);
    }
}
