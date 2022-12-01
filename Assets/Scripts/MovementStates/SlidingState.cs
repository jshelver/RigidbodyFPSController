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
        stateManager.rb.drag = stateManager.groundDrag;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();


    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        // To do: Add sliding logic
    }
}
