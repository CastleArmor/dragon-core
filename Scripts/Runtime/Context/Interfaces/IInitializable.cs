public interface IInitializable
{
    bool IsInitialized { get; }
    void InitializeIfNot();
}