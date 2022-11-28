using UnityEngine;

public class JumpState : MovementBaseState
{
    public JumpState(MovementStateManager _stateManager, MovementStateMachine _stateMachine) : base(_stateManager, _stateMachine)
    {
        stateMachine = _stateMachine;
        stateManager = _stateManager;
    }

    public override void Enter()
    {
        base.Enter();
        stateManager.rb.drag = stateManager.airDrag;
        stateManager.rb.AddForce(Vector3.up * stateManager.jumpForce, ForceMode.Impulse);
        stateManager.StartCoroutine(stateManager.JumpToFallingStateTimer());
    }
}