using UnityEngine;

namespace Dragon.Core
{
    public class State_OnEnterRaiseEvent : MonoActorState
    {
        [SerializeField] private EventField _event;

        protected override void OnEnter()
        {
            base.OnEnter();
            _event.Raise(pContext);
            FinishIfNot();
        }
    }
}