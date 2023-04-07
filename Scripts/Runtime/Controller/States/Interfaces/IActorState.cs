using System;

namespace Dragon.Core
{
    public interface IActorState : ICastable
    {
        public IActor Actor { get; }
        public IContext pContext { get; }
        bool IsRunning { get; }
        bool IsFinished { get; }
        void CheckoutEnter(IActor actor);
        void CheckoutExit();
        void CheckoutInstant(IActor main);
        event Action<IActorState> onStateFinish;
    }
}