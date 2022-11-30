using UnityEngine;
using Helper;
using System.Collections;

public class WallJumpState : MovementBaseState
{
    bool isHighEnough;
    bool isOnLeftWall, isOnRightWall;
    bool canWallRunAgain;

    public WallJumpState(MovementStateManager _stateManager, MovementStateMachine _stateMachine) : base(_stateManager, _stateMachine)
    {
        stateMachine = _stateMachine;
        stateManager = _stateManager;
    }

    public override void Enter()
    {
        base.Enter();

        canWallRunAgain = false;
        stateManager.rb.drag = stateManager.airDrag;
        UpdateWallRunBooleans();

        if (isOnLeftWall)
        {
            RaycastHit hit = WallRun.GetLeftWallHit(stateManager.rb.transform.position, -stateManager.playerOrientation.right, stateManager.maxDistanceFromWall, stateManager.wallRunnableLayers);
            stateManager.rb.AddForce(hit.normal * stateManager.wallJumpPushOffForce + stateManager.rb.transform.up * stateManager.wallJumpUpForce, ForceMode.Impulse);
        }
        else if (isOnRightWall)
        {
            RaycastHit hit = WallRun.GetRightWallHit(stateManager.rb.transform.position, stateManager.playerOrientation.right, stateManager.maxDistanceFromWall, stateManager.wallRunnableLayers);
            stateManager.rb.AddForce(hit.normal * stateManager.wallJumpPushOffForce + stateManager.rb.transform.up * stateManager.wallJumpUpForce, ForceMode.Impulse);
        }

        stateManager.StartCoroutine(WallJumpToWallRunCooldown());
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        UpdateWallRunBooleans();

        if (!isHighEnough)
        {
            // Switch to falling state
            stateMachine.ChangeState(stateManager.fallingState);
            return;
        }

        // Don't allow player to switch back to wall run state until cooldown is over
        if (!canWallRunAgain) return;

        if (isOnLeftWall || isOnRightWall)
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

    private void UpdateWallRunBooleans()
    {
        isHighEnough = WallRun.CheckIfHighEnoughOffGround(stateManager.feetTransform.position, -stateManager.transform.up, stateManager.minimumHeightToWallRun + stateManager.suspensionRestDistance, stateManager.groundLayer);

        isOnLeftWall = WallRun.CheckLeftWall(stateManager.rb.transform.position, -stateManager.playerOrientation.right, stateManager.maxDistanceFromWall, stateManager.wallRunnableLayers);
        isOnRightWall = WallRun.CheckRightWall(stateManager.rb.transform.position, stateManager.playerOrientation.right, stateManager.maxDistanceFromWall, stateManager.wallRunnableLayers);
    }

    private IEnumerator WallJumpToWallRunCooldown()
    {
        yield return new WaitForSeconds(stateManager.wallJumpToRunCooldown);
        canWallRunAgain = true;
    }

    private void HandleMovement()
    {
        Vector3 movementDir = stateManager.playerOrientation.forward * InputManager.movementInput.y + stateManager.playerOrientation.right * InputManager.movementInput.x;

        // Allow movement in air but at a reduced amount
        stateManager.rb.AddForce(movementDir * (stateManager.walkSpeed * stateManager.inAirModifier) * Time.deltaTime);
    }
}
