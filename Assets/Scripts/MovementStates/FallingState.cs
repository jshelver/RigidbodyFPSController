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
        Debug.Log("Not grounded");
        base.LogicUpdate();
        if (Suspension.CheckIfGrounded(stateManager.feetTransform, -stateManager.transform.up, stateManager.suspensionRestDistance, stateManager.groundLayer))
        {
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

            // Switch to walk state
            stateMachine.ChangeState(stateManager.walkState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
