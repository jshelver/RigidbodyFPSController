using UnityEngine;
using Helper;
using System.Collections;

public class WallRunState : MovementBaseState
{
    bool isHighEnough;
    bool isOnLeftWall, isOnRightWall;
    float yVelocity;
    RaycastHit wallHit;

    public WallRunState(MovementStateManager _stateManager, MovementStateMachine _stateMachine) : base(_stateManager, _stateMachine)
    {
        stateMachine = _stateMachine;
        stateManager = _stateManager;
    }

    public override void Enter()
    {
        base.Enter();
        yVelocity = 0;
        stateManager.rb.drag = stateManager.groundDrag;

        UpdateWallRunBooleans();

        // Invoke state manager's wall run event
        MovementStateManager.OnCameraZAngleChanged.Invoke(isOnLeftWall ? -stateManager.cameraZAngle : stateManager.cameraZAngle);
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
        if (!isOnLeftWall && !isOnRightWall)
        {
            // Switch to falling state
            stateMachine.ChangeState(stateManager.fallingState);
            return;
        }

        if (InputManager.jumpInput)
        {
            // Switch to wall jump state
            stateMachine.ChangeState(stateManager.wallJumpState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        // Get the direction we want to move in by crossing the player's forward vector with the wall's normal vector
        Vector3 wallrunDirection = Vector3.Cross(stateManager.rb.transform.up, wallHit.normal).normalized;
        // Reverse if on the left wall
        if (isOnLeftWall) wallrunDirection *= -1;

        // Move the player in the direction of the wall
        stateManager.rb.AddForce(wallrunDirection * stateManager.wallRunSpeed, ForceMode.Acceleration);

        // Keep player against the wall
        stateManager.rb.AddForce(-wallHit.normal.normalized * 10f, ForceMode.Acceleration);

        LimitVelocity();
    }

    public override void Exit()
    {
        base.Exit();
        // Invoke state manager's wall run event
        MovementStateManager.OnCameraZAngleChanged.Invoke(0f);
    }

    private void UpdateWallRunBooleans()
    {
        isHighEnough = WallRun.CheckIfHighEnoughOffGround(stateManager.feetTransform.position, -stateManager.transform.up, stateManager.minimumHeightToWallRun + stateManager.suspensionRestDistance, stateManager.groundLayer);

        isOnLeftWall = WallRun.CheckLeftWall(stateManager.rb.transform.position, -stateManager.playerOrientation.right, stateManager.maxDistanceFromWall, stateManager.wallRunnableLayers);
        isOnRightWall = WallRun.CheckRightWall(stateManager.rb.transform.position, stateManager.playerOrientation.right, stateManager.maxDistanceFromWall, stateManager.wallRunnableLayers);

        if (isOnLeftWall)
            wallHit = WallRun.GetLeftWallHit(stateManager.rb.transform.position, -stateManager.playerOrientation.right, stateManager.maxDistanceFromWall, stateManager.wallRunnableLayers);
        else if (isOnRightWall)
            wallHit = WallRun.GetRightWallHit(stateManager.rb.transform.position, stateManager.playerOrientation.right, stateManager.maxDistanceFromWall, stateManager.wallRunnableLayers);
    }

    private void LimitVelocity()
    {
        // Limit x and z velocities
        Vector3 flatVelocity = new Vector3(stateManager.rb.velocity.x, 0f, stateManager.rb.velocity.z);
        if (flatVelocity.magnitude > stateManager.maxWallRunSpeed)
        {
            flatVelocity = flatVelocity.normalized * stateManager.maxWallRunSpeed;
        }

        // Lerp player's y-velocity to 0
        yVelocity = Mathf.Lerp(yVelocity, 0f, Time.deltaTime * 1f);

        stateManager.rb.velocity = new Vector3(flatVelocity.x, yVelocity, flatVelocity.z);
    }
}