using System;

public interface IContext : IUnityObject
{
    void InitializeIfNot();
    event Action<IContext> onDestroyContext;
}