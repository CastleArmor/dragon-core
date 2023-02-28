using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReturnEventRegistry<TArg1,TReturn>
{
    private static readonly Dictionary<IEventContext, Dictionary<string,Func<TArg1,TReturn>>> _eventDictionary 
        = new Dictionary<IEventContext, Dictionary<string,Func<TArg1,TReturn>>>();

    private static readonly Dictionary<string, Func<TArg1, TReturn>> _globalEventDictionary =
        new Dictionary<string, Func<TArg1, TReturn>>();
    
    public static void Install(string key)
    {
        if (!_globalEventDictionary.ContainsKey(key))
        {
            _globalEventDictionary.Add(key,null);
        }
    }

    public static void Remove(string key)
    {
        if (_globalEventDictionary.ContainsKey(key))
        {
            _globalEventDictionary.Remove(key);
        }
    }
    
    public static void Register(string key,Func<TArg1,TReturn> action)
    {
        if (!ContainsEvent(key)) Install(key);
        _globalEventDictionary[key] += action;
    }

    public static void Unregister(string key,Func<TArg1,TReturn> action)
    {
        if (!ContainsEvent(key)) return;
        _globalEventDictionary[key] -= action;
    }

    public static TReturn Raise(string key,TArg1 arg1)
    {
        if (!ContainsEvent(key)) return default;
        return _globalEventDictionary[key].Invoke(arg1);
    }

    public static void Install(IEventContext main, string key)
    {
        if (_eventDictionary.ContainsKey(main))
        {
            if (_eventDictionary[main].ContainsKey(key)) return;
            
            _eventDictionary[main].Add(key,null);
        }
        else
        {
            _eventDictionary.Add(main,new Dictionary<string, Func<TArg1,TReturn>>(){{key,null}});
            main.onDestroyEventContext += OnRequestRemoveData;
        }
    }

    private static void OnRequestRemoveData(IEventContext obj)
    {
        _eventDictionary[obj] = null;
        _eventDictionary.Remove(obj);
        obj.onDestroyEventContext -= OnRequestRemoveData;
    }
    
    public static void Register(IEventContext main, string key,Func<TArg1,TReturn> action)
    {
        if(!ContainsEvent(main,key)) Install(main,key);
        _eventDictionary[main][key] += action;
    }

    public static void Unregister(IEventContext main, string key,Func<TArg1,TReturn> action)
    {
        if (!ContainsEvent(main, key)) return;
        _eventDictionary[main][key] -= action;
    }

    public static TReturn Raise(IEventContext main, string key,TArg1 arg1)
    {
        if (!ContainsEvent(main,key)) return default;
        if (_eventDictionary[main][key] != null)
        {
            return _eventDictionary[main][key].Invoke(arg1);
        }
        else return default;
    }
    
    public static TReturn TryRaise(IEventContext main, string key,TArg1 arg1)
    {
        if (!ContainsEvent(main,key)) return default;
        return _eventDictionary[main][key].Invoke(arg1);
    }

    public static bool ContainsEvent(IEventContext main, string key)
    {
        if (!_eventDictionary.ContainsKey(main)) return false;
        if (!_eventDictionary[main].ContainsKey(key)) return false;
        return true;
    }
    
    public static bool ContainsEvent(string key)
    {
        if (!_globalEventDictionary.ContainsKey(key)) return false;
        return true;
    }
}
