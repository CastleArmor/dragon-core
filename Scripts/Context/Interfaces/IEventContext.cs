using System;

public interface IEventContext : IContext, IUnityObject
{
    event Action<IEventContext> onDestroyEventContext;
}