using System.Collections;
using UnityEngine;
using Helper;

public class JumpState : MovementBaseState
{
    bool readyToWallRun;

    public JumpState(MovementStateManager _stateManager, MovementStateMachine _stateMachine) : base(_stateManager, _stateMachine)
    {
        stateMachine = _stateMachine;
        stateManager = _stateManager;
    }

    public override void Enter()
    {
        base.Enter();
        readyToWallRun = false;
        // Set drag to air drag so momentum in carried in air
        stateManager.rb.drag = stateManager.airDrag;
        //Reset y-velocity if negative so you can properly jump going down slopes
        stateManager.rb.velocity = new Vector3(stateManager.rb.velocity.x, stateManager.rb.velocity.y > 0 ? stateManager.rb.velocity.y : 0f, stateManager.rb.velocity.z);
        // Add jump force
        stateManager.rb.AddForce(Vector3.up * stateManager.jumpForce, ForceMode.Impulse);
        stateManager.StartCoroutine(JumpToFallingStateTimer());

        // Start wall run cooldown
        stateManager.StartCoroutine(WallRunCooldown());
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Check if we are close enough to a wall to wall run
        bool isWallWithinDistance = WallRun.CheckLeftWall(stateManager.rb.transform.position, -stateManager.playerOrientation.right, stateManager.maxDistanceFromWall, stateManager.wallRunnableLayers)
            || WallRun.CheckRightWall(stateManager.rb.transform.position, stateManager.playerOrientation.right, stateManager.maxDistanceFromWall, stateManager.wallRunnableLayers);

        // Check if we are high enough off the ground to wall run
        bool isHighEnough = WallRun.CheckIfHighEnoughOffGround(stateManager.feetTransform.position, -stateManager.transform.up, stateManager.minimumHeightToWallRun + stateManager.suspensionRestDistance, stateManager.groundLayer);

        if (readyToWallRun && isWallWithinDistance && isHighEnough)
        {
            // Switch to wall run state
            stateMachine.ChangeState(stateManager.wallRunState);
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

        // Allow movement in air but at a reduced amount
        stateManager.rb.AddForce(movementDir * (stateManager.walkSpeed * stateManager.inAirModifier) * Time.deltaTime);
    }

    private IEnumerator JumpToFallingStateTimer()
    {
        yield return new WaitForSeconds(0.1f);
        // wait for unknown seconds then switch to falling state
        for (float i = 0; i < stateManager.jumpToFallingTimeDelay; i += Time.deltaTime)
        {
            if (Helper.Suspension.CheckIfGrounded(stateManager.feetTransform, -stateManager.transform.up, stateManager.suspensionRestDistance, stateManager.groundLayer))
            {
                if (InputManager.movementInput == Vector2.zero)
                {
                    // Switch to idle state
                    stateMachine.ChangeState(stateManager.idleState);
                    yield break;
                }

                if (InputManager.runInput)
                {
                    // Switch to run state
                    stateMachine.ChangeState(stateManager.runState);
                    yield break;
                }

                // Switch to walk state
                stateMachine.ChangeState(stateManager.walkState);
                yield break;
            }
            if (stateMachine.currentState != stateManager.jumpState)
            {
                // Stops the coroutine if the state changes
                yield break;
            }
            yield return null;
        }
        stateMachine.ChangeState(stateManager.fallingState);
    }

    private IEnumerator WallRunCooldown()
    {
        yield return new WaitForSeconds(0.2f);
        readyToWallRun = true;
    }
}