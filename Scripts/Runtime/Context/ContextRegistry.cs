using System.Collections.Generic;

namespace Dragon.Core
{
    public static class ContextRegistry
    {
        private static readonly Dictionary<string, IContext> _idToContext = new Dictionary<string, IContext>();
        private static readonly Dictionary<IContext, string> _contextToID = new Dictionary<IContext, string>();
    
        public static IContext GetContext(string instanceID)
        {
            return _idToContext[instanceID];
        }
    
        public static string GetID(IContext context)
        {
            return _contextToID[context];
        }

        public static bool Contains(string instanceID)
        {
            return _idToContext.ContainsKey(instanceID);
        }
    
        public static bool Contains(IContext instanceID)
        {
            return _contextToID.ContainsKey(instanceID);
        }

        public static void Set(IContext context)
        {
            string stringID = context.ContextID;
            _idToContext[stringID] = context;
            _contextToID[context] = stringID;
        }

        public static void Set(string contextID, IContext context)
        {
            string stringID = contextID;
            _idToContext[stringID] = context;
            _contextToID[context] = stringID;
        }

        public static void Remove(IContext context)
        {
            string stringID = context.ContextID;
            _idToContext.Remove(stringID);
            _contextToID.Remove(context);
        }
    }
}