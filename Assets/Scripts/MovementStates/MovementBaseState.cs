using UnityEngine;
using Helper;

public class MovementBaseState
{
    protected MovementStateManager stateManager;
    protected MovementStateMachine stateMachine;

    public MovementBaseState(MovementStateManager _stateManager, MovementStateMachine _stateMachine)
    {
        stateManager = _stateManager;
        stateMachine = _stateMachine;
    }

    public virtual void Enter()
    {
        string stateName = this.ToString();
        Debug.Log($"Enter {stateName} State");
    }

    public virtual void LogicUpdate()
    {
        // Smoothdamp player height no matter the state (allows for smooth transitions between in and out of crouch/sliding state)
        stateManager.currentHeight = Mathf.SmoothDamp(stateManager.currentHeight, stateManager.targetHeight, ref stateManager.heightChangeVelocity, stateManager.heightChangeSmoothTime);
        stateManager.rb.transform.localScale = new Vector3(1f, stateManager.currentHeight, 1f);
    }

    public virtual void PhysicsUpdate()
    {
        if (Physics.Raycast(stateManager.feetTransform.position, -stateManager.transform.up, out RaycastHit hit, stateManager.suspensionRestDistance, stateManager.groundLayer))
        {
            Suspension.ApplySuspensionForce(stateManager.rb, hit, stateManager.springStrength, stateManager.springDamper, stateManager.suspensionRestDistance);
        }
    }

    public virtual void Exit()
    {

    }
}
