using System;

public interface IContext : IUnityObject
{
    string InstanceID { get; }
    void InitializeIfNot();
    event Action<IContext> onDestroyContext;
}