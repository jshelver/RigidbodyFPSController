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
        // Set drag to air drag so momentum in carried in air
        stateManager.rb.drag = stateManager.airDrag;
        //Reset y-velocity if negative so you can properly jump going down slopes
        stateManager.rb.velocity = new Vector3(stateManager.rb.velocity.x, stateManager.rb.velocity.y > 0 ? stateManager.rb.velocity.y : 0f, stateManager.rb.velocity.z);
        // Add jump force
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

        // Allow movement in air but at a reduced amount
        stateManager.rb.AddForce(movementDir * (stateManager.walkSpeed * stateManager.inAirModifier) * Time.deltaTime);
    }
}