using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventRegistry<TArg1>
{
    private static readonly Dictionary<IEventContext, Dictionary<string,Action<EventArgs,TArg1>>> _eventDictionary 
        = new Dictionary<IEventContext, Dictionary<string,Action<EventArgs,TArg1>>>();

    private static readonly Dictionary<string, Action<EventArgs,TArg1>> _globalEventDictionary =
        new Dictionary<string, Action<EventArgs,TArg1>>();

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

    public static void Register(string key,Action<EventArgs,TArg1> action)
    {
        if (!ContainsEvent(key)) Install(key);
        _globalEventDictionary[key] += action;
    }

    public static void Unregister(string key,Action<EventArgs,TArg1> action)
    {
        _globalEventDictionary[key] -= action;
    }

    public static void Raise(string key,TArg1 arg1)
    {
        if (!ContainsEvent(key)) return;
        _globalEventDictionary[key]?.Invoke(new EventArgs(){EventContext = null,EventName = key},arg1);
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
            _eventDictionary.Add(main,new Dictionary<string, Action<EventArgs,TArg1>>(){{key,null}});
            main.onDestroyEventContext += OnRequestRemoveData;
        }
    }

    private static void OnRequestRemoveData(IEventContext obj)
    {
        _eventDictionary[obj] = null;
        _eventDictionary.Remove(obj);
        obj.onDestroyEventContext -= OnRequestRemoveData;
    }

    public static void Register(IEventContext main, string key,Action<EventArgs,TArg1> action)
    {
        if(!ContainsEvent(main,key)) Install(main,key);
        _eventDictionary[main][key] += action;
    }

    public static void Unregister(IEventContext main, string key,Action<EventArgs,TArg1> action)
    {
        _eventDictionary[main][key] -= action;
    }

    public static void Raise(IEventContext main, string key,TArg1 arg1)
    {
        if (!ContainsEvent(main,key)) return;
        _eventDictionary[main][key]?.Invoke(new EventArgs(){EventContext = main,EventName = key},arg1);
    }
    
    public static void TryRaise(IEventContext main, string key,TArg1 arg1)
    {
        if (!ContainsEvent(main,key)) return;
        _eventDictionary[main][key]?.Invoke(new EventArgs(){EventContext = main,EventName = key},arg1);
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
