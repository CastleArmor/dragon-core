using System.Collections.Generic;

namespace Dragon.Core
{
    public static class EventContextRegistry
    {
        private static readonly Dictionary<string, IEventContext> _idToContext = new Dictionary<string, IEventContext>();
        private static readonly Dictionary<IEventContext, string> _contextToID = new Dictionary<IEventContext, string>();
    
        public static IEventContext GetContext(string instanceID)
        {
            return _idToContext[instanceID];
        }
    
        public static string GetID(IEventContext context)
        {
            return _contextToID[context];
        }

        public static void Set(IEventContext context)
        {
            string stringID = context.ContextID;
            _idToContext[stringID] = context;
            _contextToID[context] = stringID;
        }

        public static void Set(string contextID, IEventContext context)
        {
            string stringID = contextID;
            _idToContext[stringID] = context;
            _contextToID[context] = stringID;
        }

        public static void Remove(IEventContext context)
        {
            string stringID = context.ContextID;
            _idToContext.Remove(stringID);
            _contextToID.Remove(context);
        }
    }
}