using UnityEngine;
using Helper;
using System.Collections;

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
            if (InputManager.runInput && InputManager.movementInput.y > 0f)
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

        // If the player is not grounded, then start timer to switch to falling state
        if (!Suspension.CheckIfGrounded(stateManager.feetTransform, -stateManager.transform.up, stateManager.suspensionRestDistance, stateManager.groundLayer))
        {
            stateManager.StartCoroutine(GroundedToFallingStateTimer());
            return;
        }

        if (InputManager.crouchInput)
        {
            // Switch to crouch state
            stateMachine.ChangeState(stateManager.crouchState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
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
