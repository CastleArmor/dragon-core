using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public struct DataOnChangeArgs<T>
{
    public string AssignedKey;
    public IContext Context;
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

    private static IChangeEvaluator<T> _equalityComparer;

    private static bool _isRegisteredToAppQuit;
    private static bool _isRegisteredToAppPause;
    private static bool _recievedChangeEvaluator;
    
    private static StringBuilder _stringBuilder = new StringBuilder();
    
    private static void OnApplicationQuit()
    {
        foreach (string key in _globalKeys)
        {
            if (!Dictionary.ContainsKey(key)) continue;
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
            _equalityComparer = ChangeEvaluatorFactory<T>.GetChangeEvaluator();
        }
    }

    public static void BindData(IContext context,T value, string key)
    {
        EnsureEventRegisters();

        string assignedID = GetAssignedKey(context, key);
        if (context != null)
        {
            context.onDestroyContext += (c) => UnbindData(c,key);
        }

        Dictionary[assignedID] = value;
    }

    public static void UnbindData(IContext context,string key)
    {
        EnsureEventRegisters();
        
        string assignedID = GetAssignedKey(context, key);
        if (context != null)
        {
            context.onDestroyContext -= (c) => UnbindData(c,key);
        }
        
        Dictionary.Remove(assignedID);
    }

    private static string GetAssignedKey(IContext context,string key)
    {
        _stringBuilder.Clear();
        _stringBuilder.Append("Global/");
        _stringBuilder.Append(key);
        string assignedID = _stringBuilder.ToString();
        if (context == null)
        {
            if (!_globalKeys.Contains(assignedID))
            {
                _globalKeys.Add(assignedID);
            }
        }
        else
        {
            if (!ContextRegistry.Contains(context))
            {
                return assignedID;
            }
            string contextID = ContextRegistry.GetID(context);
            _stringBuilder.Clear();
            _stringBuilder.Append(contextID);
            _stringBuilder.Append("/");
            _stringBuilder.Append(key);
            assignedID = _stringBuilder.ToString();
        }

        return assignedID;
    }

    public static void RemoveData(IContext context, string key = "")
    {
        EnsureEventRegisters();
        string assignedID = GetAssignedKey(context, key);
        if (context == null)
        {
            if (!_globalKeys.Contains(assignedID))
            {
                return;
            }
        }
        else
        {
            string contextID = ContextRegistry.GetID(context);
            if (!_contextKeys.ContainsKey(contextID))
            {
                return;
            }

            string givenKey = "Single";
            if (!string.IsNullOrEmpty(key))
            {
                givenKey = key;
            }
            _contextKeys[contextID].Remove(givenKey);

            if (_contextKeys.Count == 0)
            {
                _contextKeys.Remove(contextID);
                context.onDestroyContext -= OnDestroyContextOfInstalledData;
            }
        }
        if (!Dictionary.ContainsKey(assignedID))
        {
            return;
        }
        T oldValue = Dictionary[assignedID];
        if (oldValue is IInstalledData installedData)
        {
            installedData.OnRemoveData();
        }
        if (_onChangeDictionary.ContainsKey(assignedID))
        {
            Debug.Log("Contains Assigned ID = " + assignedID);
            if (!_equalityComparer.Equals(oldValue, default))
            {
                Debug.Log("OnChanged Assigned ID = " + assignedID);
                _onChangeDictionary[assignedID].Invoke(new DataOnChangeArgs<T>()
                {
                    AssignedKey = assignedID,
                    Context = context,
                    OldValue = oldValue,
                    NewValue = default
                });
            }
        }
    }

    public static void SetData(IContext context, T value, string key = "")
    {
        EnsureEventRegisters();

        string assignedID = GetAssignedKey(context, key);
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
            }

            string givenKey = "Single";
            if (!string.IsNullOrEmpty(key))
            {
                givenKey = key;
            }
            _contextKeys[contextID].Add(givenKey);
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
            installedData.KeyID = key;
            installedData.OnInstalledData(context);
        }

        if (_onChangeDictionary.ContainsKey(assignedID))
        {
            Debug.Log("Contains Assigned ID = " + assignedID);
            if (!_equalityComparer.Equals(oldValue, value))
            {
                Debug.Log("OnChanged Assigned ID = " + assignedID);
                _onChangeDictionary[assignedID]?.Invoke(new DataOnChangeArgs<T>()
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

    public static bool ContainsData(IContext context, string key)
    {
        string assignedID = GetAssignedKey(context, key);
        return Dictionary.ContainsKey(assignedID);
    }
    
    public static bool ContainsData(string contextID, string key)
    {
        _stringBuilder.Clear();
        _stringBuilder.Append(contextID);
        _stringBuilder.Append("/");
        _stringBuilder.Append(key);
        string assignedID = _stringBuilder.ToString();
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

    public static T GetData(IContext context, string key = "")
    {
        string assignedID = GetAssignedKey(context, key);
        #if UNITY_EDITOR
        if (!Dictionary.ContainsKey(assignedID))
        {
            Debug.LogError(context.As<IUnityObject>().name + " doesn't contain " + typeof(T).Name + " With " + (string.IsNullOrEmpty(key)?"Single":key));
        }
        #endif
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
        foreach (string dataKey in _contextKeys[ContextRegistry.GetID(context.As<IDataContext>())])
        {
            if (!Dictionary.ContainsKey(dataKey)) continue;
            if (Dictionary[dataKey] is IInstalledData installedData)
            {
                installedData.OnRemoveData();
            }
            Dictionary.Remove(dataKey);
        }
    }

    public static void RegisterOnChange(IContext context, Action<DataOnChangeArgs<T>> action,string key = "")
    {
        string assignedID = GetAssignedKey(context, key);

        if (!_onChangeDictionary.ContainsKey(assignedID))
        {
            _onChangeDictionary.Add(assignedID,null);
        }

        _onChangeDictionary[assignedID] += action;
    }
    
    public static void UnregisterOnChange(IContext context, Action<DataOnChangeArgs<T>> action,string key = "")
    {
        string assignedID = GetAssignedKey(context, key);

        if (!_onChangeDictionary.ContainsKey(assignedID))
        {
            return;
        }

        _onChangeDictionary[assignedID] -= action;
    }
}