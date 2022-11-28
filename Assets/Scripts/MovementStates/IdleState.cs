using UnityEngine;
using Helper;

public class IdleState : MovementBaseState
{

    public IdleState(MovementStateManager _stateManager, MovementStateMachine _stateMachine) : base(_stateManager, _stateMachine)
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
        if (InputManager.movementInput != Vector2.zero)
        {
            if (InputManager.runInput)
            {
                // Switch to run state
                stateMachine.ChangeState(stateManager.runState);
                return;
            }
            else
            {
                // Switch to walk state
                stateMachine.ChangeState(stateManager.walkState);
                return;
            }
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
    }
}
