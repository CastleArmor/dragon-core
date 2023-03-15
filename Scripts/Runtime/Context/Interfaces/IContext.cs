using System;

namespace Dragon.Core
{
    public interface IContext : ICastable
    {
        string ContextID { get; }
        void InitializeIfNot();
        event Action<IContext> onDestroyContext;
    }
}