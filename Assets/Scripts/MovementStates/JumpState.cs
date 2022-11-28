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