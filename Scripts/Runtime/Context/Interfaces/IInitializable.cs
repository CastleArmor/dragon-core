namespace Dragon.Core
{
    public interface IInitializable
    {
        bool IsInitialized { get; }
        void InitializeIfNot();
        void FinalizeIfNot();
    }
}