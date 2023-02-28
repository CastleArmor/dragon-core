using UnityEngine;

public class State_OnExitRaiseEvent : MonoActorState
{
    [SerializeField] private EventField _event;

    protected override void OnEnter()
    {
        base.OnEnter();
        FinishIfNot();
    }
    

    protected override void OnExit()
    {
        base.OnExit();
        _event.Raise(EventContext);
    }
}