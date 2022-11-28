public class MovementStateMachine
{
    public MovementBaseState currentState { get; private set; }

    public void Initialize(MovementBaseState startingState)
    {
        currentState = startingState;
        currentState.Enter();
    }

    public void ChangeState(MovementBaseState newState)
    {
        currentState.Exit();

        currentState = newState;
        currentState.Enter();
    }
}
