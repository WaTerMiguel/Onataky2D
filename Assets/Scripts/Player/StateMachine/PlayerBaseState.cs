public abstract class PlayerBaseState
{
    protected PlayerController _ctx;
    protected PlayerStateFactory _factory;
    
    public PlayerBaseState(PlayerController currentContext, PlayerStateFactory playerStateFactory)
    {
        _ctx = currentContext;
        _factory = playerStateFactory;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwichStates();

    protected void SwitchState(PlayerBaseState newState)
    {
        // Exit current state
        ExitState();

        // Enter new state
        newState.EnterState();

        // Change current state to new state
        _ctx.CurrentState = newState;

    }
}
