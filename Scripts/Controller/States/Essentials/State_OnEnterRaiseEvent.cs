using UnityEngine;

public class State_OnEnterRaiseEvent : MonoActorState
{
    [SerializeField] private EventField _event;

    protected override void OnEnter()
    {
        base.OnEnter();
        _event.Raise(EventContext);
        FinishIfNot();
    }
}