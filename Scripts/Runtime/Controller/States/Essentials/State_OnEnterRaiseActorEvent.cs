using UnityEngine;

namespace Dragon.Core
{
    public class State_OnEnterRaiseActorEvent : MonoActorState
    {
        [SerializeField] private AddressField _actorToSend;
        [SerializeField] private EventField<IActor> _event;

        protected override void OnEnter()
        {
            base.OnEnter();
            _event.Raise(pContext,_actorToSend.GetFromAddress(pContext).GetActor());
            FinishIfNot();
        }
    }
}