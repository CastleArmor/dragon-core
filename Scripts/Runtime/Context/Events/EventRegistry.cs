using System;
using System.Collections.Generic;

namespace Dragon.Core
{
    public static class EventRegistry
    {
        private static readonly Dictionary<IContext, Dictionary<string,Action<EventArgs>>> _eventDictionary 
            = new Dictionary<IContext, Dictionary<string,Action<EventArgs>>>();

        private static readonly Dictionary<string, Action<EventArgs>> _globalEventDictionary =
            new Dictionary<string, Action<EventArgs>>();

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

        public static Action<EventArgs> Register(string key,Action<EventArgs> action)
        {
            if (!ContainsEvent(key)) Install(key);
            _globalEventDictionary[key] += action;
            return _globalEventDictionary[key];
        }

        public static Action<EventArgs> Unregister(string key,Action<EventArgs> action)
        {
            _globalEventDictionary[key] -= action;
            return _globalEventDictionary[key];
        }

        public static void Raise(string key)
        {
            if (!ContainsEvent(key)) return;
            _globalEventDictionary[key]?.Invoke(new EventArgs(){EventContext = null,EventName = key});
        }

        public static void Install(IContext main, string key)
        {
            if (_eventDictionary.ContainsKey(main))
            {
                if (_eventDictionary[main].ContainsKey(key)) return;
        
                _eventDictionary[main].Add(key,null);
            }
            else
            {
                _eventDictionary.Add(main,new Dictionary<string, Action<EventArgs>>(){{key,null}});
                main.onDestroyContext += OnRequestRemoveData;
            }
        }

        private static void OnRequestRemoveData(IContext obj)
        {
            _eventDictionary[obj] = null;
            _eventDictionary.Remove(obj);
            obj.onDestroyContext -= OnRequestRemoveData;
        }

        public static Action<EventArgs> Register(IContext main, string key,Action<EventArgs> action)
        {
            if(!ContainsEvent(main,key)) Install(main,key);
            _eventDictionary[main][key] += action;
            return _eventDictionary[main][key];
        }

        public static Action<EventArgs> Unregister(IContext main, string key,Action<EventArgs> action)
        {
            if(!ContainsEvent(main,key)) return null;
            _eventDictionary[main][key] -= action;
            return _eventDictionary[main][key];
        }

        public static void Raise(IContext main, string key)
        {
            if (!ContainsEvent(main,key)) return;
            _eventDictionary[main][key]?.Invoke(new EventArgs(){EventContext = main,EventName = key});
        }
    
        public static void TryRaise(IContext main, string key)
        {
            if (!ContainsEvent(main,key)) return;
            _eventDictionary[main][key]?.Invoke(new EventArgs(){EventContext = main,EventName = key});
        }

        public static bool ContainsEvent(IContext main, string key)
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
}
    

