using System;
using System.Collections.Generic;
using UnityEngine;

public struct DataOnChangeArgs<T>
{
    public string AssignedKey;
    public IDataContext Context;
    public T OldValue;
    public T NewValue;
}

public static class DataRegistry<T>
{
    private static readonly Dictionary<string, T> _dictionary = new Dictionary<string, T>();
    public static Dictionary<string, T> Dictionary => _dictionary;

    private static readonly Dictionary<string, List<string>> _contextKeys =
        new Dictionary<string, List<string>>();

    private static readonly Dictionary<string, Action<DataOnChangeArgs<T>>> _onChangeDictionary =
        new Dictionary<string, Action<DataOnChangeArgs<T>>>();

    private static readonly HashSet<string> _globalKeys = new HashSet<string>();

    private static IEqualityComparer<T> _equalityComparer;

    private static bool _isRegisteredToAppQuit;
    private static bool _isRegisteredToAppPause;
    private static bool _recievedChangeEvaluator;
    
    private static void OnApplicationQuit()
    {
        foreach (string key in _globalKeys)
        {
            if (Dictionary[key] is IInstalledData installedData)
            {
                installedData.OnRemoveData();
            }
            Dictionary.Remove(key);
        }
        _globalKeys.Clear();
        StaticUpdate.onApplicationQuit -= OnApplicationQuit;
        StaticUpdate.onApplicationPause -= OnApplicationPause;
        _isRegisteredToAppQuit = false;
        _isRegisteredToAppPause = false;
    }

    private static void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus) return;
        foreach (string key in _globalKeys)
        {
            if (Dictionary[key] is ISaveableData saveable)
            {
                saveable.SaveData();
            }
        }
    }

    private static void EnsureEventRegisters()
    {
        if (!_isRegisteredToAppQuit)
        {
            StaticUpdate.onApplicationQuit += OnApplicationQuit;
            _isRegisteredToAppQuit = true;
        }

        if (!_isRegisteredToAppPause)
        {
            StaticUpdate.onApplicationPause += OnApplicationPause;
            _isRegisteredToAppPause = true;
        }

        if (!_recievedChangeEvaluator)
        {
            
        }
    }

    public static void BindData(IDataContext context, T value, string key)
    {
        EnsureEventRegisters();
        
        Dictionary[key] = value;
    }

    public static void UnbindData(IDataContext context, string key)
    {
        EnsureEventRegisters();
        Dictionary.Remove(key);
    }

    private static void OnDestroyContextOfBoundData(IContext context)
    {
        context.onDestroyContext -= OnDestroyContextOfBoundData;
        foreach (string dataKey in _contextKeys[ContextRegistry.GetID(context)])
        {
            Dictionary.Remove(dataKey);
        }
    }

    public static void SetData(IDataContext context, T value, string key = "")
    {
        EnsureEventRegisters();

        string assignedID = "Global/"+key;
        if (context == null)
        {
            if (!_globalKeys.Contains(assignedID))
            {
                _globalKeys.Add(assignedID);
            }
        }
        else
        {
            string contextID = ContextRegistry.GetID(context);
            if (!_contextKeys.ContainsKey(contextID))
            {
                _contextKeys.Add(contextID,new List<string>());
                context.onDestroyContext += OnDestroyContextOfInstalledData;

                assignedID = ContextRegistry.GetID(context);
                assignedID += "/" + key;
            }
        }

        T oldValue = default;
        if (Dictionary.ContainsKey(assignedID))
        {
            oldValue = Dictionary[assignedID];
        }
        Dictionary[assignedID] = value;
        if (value is IInstalledData installedData)
        {
            installedData.AssignedID = assignedID;
            installedData.OnInstalledData(context);
        }

        if (_onChangeDictionary.ContainsKey(assignedID))
        {
            if (!_equalityComparer.Equals(oldValue, value))
            {
                _onChangeDictionary[assignedID].Invoke(new DataOnChangeArgs<T>()
                {
                    AssignedKey = assignedID,
                    Context = context,
                    OldValue = oldValue,
                    NewValue = value
                });
            }
        }
    }

    public static bool ContainsData(string fullKey)
    {
        return Dictionary.ContainsKey(fullKey);
    }

    public static bool ContainsData(IDataContext context, string key)
    {
        string assignedID = "Global/"+key;
        if(context != null)
        {
            assignedID = ContextRegistry.GetID(context);
            assignedID += "/" + key;
        }

        return Dictionary.ContainsKey(assignedID);
    }
    
    public static bool ContainsData(string contextID, string key)
    {
        string assignedID = contextID+"/"+key;
        return Dictionary.ContainsKey(assignedID);
    }

    public static bool TryGetData(IDataContext context, out T data, string key = "")
    {
        if (ContainsData(context, key))
        {
            data = GetData(context, key);
            return true;
        }

        data = default;
        return false;
    }

    public static void TryActionOnData(IDataContext context, Action<T> action, string key = "")
    {
        if (ContainsData(context, key))
        {
            action?.Invoke(GetData(context,key));
        }
    }
    
    public static bool TryGetDataByID(string contextID, out T data, string key = "")
    {
        if (ContainsData(contextID, key))
        {
            data = GetDataByID(contextID, key);
            return true;
        }

        data = default;
        return false;
    }

    public static void TryActionOnDataByID(string contextID, Action<T> action, string key = "")
    {
        if (ContainsData(contextID, key))
        {
            action?.Invoke(GetDataByID(contextID,key));
        }
    }

    public static T GetData(IDataContext context, string key = "")
    {
        string assignedID = "Global/"+key;
        if(context != null)
        {
            assignedID = ContextRegistry.GetID(context);
            assignedID += "/" + key;
        }
        return Dictionary[assignedID];
    }
    
    public static T GetDataByID(string contextID, string key = "")
    {
        string assignedID = contextID+"/"+key;
        return Dictionary[assignedID];
    }

    private static void OnDestroyContextOfInstalledData(IContext context)
    {
        context.onDestroyContext -= OnDestroyContextOfInstalledData;
        foreach (string dataKey in _contextKeys[ContextRegistry.GetID(context)])
        {
            if (!Dictionary.ContainsKey(dataKey)) continue;
            if (Dictionary[dataKey] is IInstalledData installedData)
            {
                installedData.OnRemoveData();
            }
            Dictionary.Remove(dataKey);
        }
    }

    public static void RegisterOnChange(IDataContext context, Action<DataOnChangeArgs<T>> action,string key = "")
    {
        string assignedID = "Global/"+key;
        if(context != null)
        {
            assignedID = ContextRegistry.GetID(context);
            assignedID += "/" + key;
        }

        if (!_onChangeDictionary.ContainsKey(assignedID))
        {
            _onChangeDictionary.Add(assignedID,null);
        }

        _onChangeDictionary[assignedID] += action;
    }
    
    public static void UnregisterOnChange(IDataContext context, Action<DataOnChangeArgs<T>> action,string key = "")
    {
        string assignedID = "Global/"+key;
        if(context != null)
        {
            assignedID = ContextRegistry.GetID(context);
            assignedID += "/" + key;
        }

        if (!_onChangeDictionary.ContainsKey(assignedID))
        {
            _onChangeDictionary.Add(assignedID,null);
        }

        _onChangeDictionary[assignedID] -= action;
    }
}