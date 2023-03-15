using System;

namespace Dragon.Core
{
    public interface IEventContext : IContext, IUnityObject
    {
        event Action<IEventContext> onDestroyEventContext;
    }
}