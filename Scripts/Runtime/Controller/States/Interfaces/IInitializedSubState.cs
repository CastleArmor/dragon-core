

namespace Dragon.Core
{
    public interface IInitializedSubState : IActorState
    {
        bool IsInitialized { get; }
        void Initialize();
    }
}