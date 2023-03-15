public static class _DCoreStateExtensions
{
    public static void TryInitializeIfInitializedSubState(this IActorState actorState)
    {
        if (actorState is IInitializedSubState initialized)
        {
            if(!initialized.IsInitialized) initialized.Initialize();
        }
    }

    public static void CheckoutIf(this StateField field,IActor main, bool value)
    {
        if (value)
        {
            if (field.State == null) return;
            if (field.State.IsRunning) return;
            field.State.CheckoutEnter(main);
        }
        else
        {
            if (field.State == null) return;
            if (!field.State.IsRunning) return;
            field.State.CheckoutExit();
        }
    }
}