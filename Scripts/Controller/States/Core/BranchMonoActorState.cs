using Sirenix.OdinInspector;

public abstract class BranchMonoActorState : InitializedMonoActorState
{
    [ShowInInspector][ReadOnly]
    private IActorState _currentState;
    protected abstract IActorState InitialState {get;}

    protected override void OnEnter()
    {
        base.OnEnter();
        _currentState = InitialState;
        if (_currentState != null)
        {
            _currentState.CheckoutEnter(Actor);
        }

        OnAfterInitialStateEntered();
    }

    protected virtual void OnAfterInitialStateEntered()
    {
            
    }

    protected void SwitchStateIfNotEntered(IActorState toState)
    {
        if (_currentState == toState) return;
        if (_currentState != null)
        {
            _currentState.CheckoutExit();
        }
        _currentState = toState;
        if (_currentState != null)
        {
            _currentState.CheckoutEnter(Actor);
        }
    }

    protected override void OnExit()
    {
        base.OnExit();
        _currentState.CheckoutExit();
        _currentState = null;
    }
}