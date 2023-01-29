using System.Collections.Generic;

public static class ContextRegistry
{
    private static readonly Dictionary<string, IContext> _idToContext = new Dictionary<string, IContext>();
    private static readonly Dictionary<IContext, string> _contextToID = new Dictionary<IContext, string>();

    public static IContext GetContext(int instanceID)
    {
        return _idToContext[instanceID.ToString()];
    }
    
    public static string GetID(IContext context)
    {
        return _contextToID[context];
    }

    public static void Set(IContext context)
    {
        string stringID = context.InstanceID;
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
        string stringID = context.InstanceID;
        _idToContext.Remove(stringID);
        _contextToID.Remove(context);
    }
}