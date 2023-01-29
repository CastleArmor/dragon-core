public abstract class InitializedMonoActorState : MonoActorState,IInitializedSubState
{
    public override StateFlags Flags
    {
        get
        {
            StateFlags flags = base.Flags;
            if (_isInitialized)
            {
                flags |= StateFlags.Initialized;
            }
            return flags;
        }
    }

    private bool _isInitialized;
    public bool IsInitialized => _isInitialized;
    public void Initialize()
    {
        if (_isInitialized) return;
        OnInitialize();
        _isInitialized = true;
    }

    protected abstract void OnInitialize();

    private void OnDestroy()
    {
        if (_isInitialized)
        {
            OnDestroyInitialized();
        }
    }

    protected virtual void OnDestroyInitialized()
    {
            
    }
}