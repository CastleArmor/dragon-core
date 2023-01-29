using System.Collections.Generic;

public static class DataContextRegistry
{
    private static readonly Dictionary<string, IDataContext> _idToContext = new Dictionary<string, IDataContext>();
    private static readonly Dictionary<IDataContext, string> _contextToID = new Dictionary<IDataContext, string>();
    
    public static IDataContext GetContext(string instanceID)
    {
        return _idToContext[instanceID];
    }
    
    public static string GetID(IDataContext context)
    {
        return _contextToID[context];
    }

    public static void Set(IDataContext context)
    {
        string stringID = context.DataContextID;
        _idToContext[stringID] = context;
        _contextToID[context] = stringID;
    }

    public static void Set(string contextID, IDataContext context)
    {
        string stringID = contextID;
        _idToContext[stringID] = context;
        _contextToID[context] = stringID;
    }

    public static void Remove(IDataContext context)
    {
        string stringID = context.DataContextID;
        _idToContext.Remove(stringID);
        _contextToID.Remove(context);
    }
}