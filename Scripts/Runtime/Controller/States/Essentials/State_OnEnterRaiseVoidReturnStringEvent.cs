using UnityEngine;

namespace Dragon.Core
{
    public class State_OnEnterRaiseVoidReturnStringEvent : MonoActorState
    {
        [SerializeField] private ReturnEventField<string> _event;
        protected override void OnEnter()
        {
            base.OnEnter();
            _event.Raise(EventContext);
        }
    }
}