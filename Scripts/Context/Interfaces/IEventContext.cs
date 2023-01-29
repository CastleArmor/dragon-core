using System;

public interface IEventContext : IContext
{
    string EventContextID { get; }
    event Action<IEventContext> onDestroyEventContext;
}