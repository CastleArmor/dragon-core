using System;
using System.Collections.Generic;

namespace Dragon.Core
{
    public static class ReturnEventRegistry<TReturn>
    {
        private static readonly Dictionary<IContext, Dictionary<string,Func<EventArgs,TReturn>>> _eventDictionary 
            = new Dictionary<IContext, Dictionary<string,Func<EventArgs,TReturn>>>();

        private static readonly Dictionary<string, Func<EventArgs,TReturn>> _globalEventDictionary =
            new Dictionary<string, Func<EventArgs,TReturn>>();
    
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
    
        public static void Register(string key,Func<EventArgs,TReturn> action)
        {
            if (!ContainsEvent(key)) Install(key);
            _globalEventDictionary[key] += action;
        }

        public static void Unregister(string key,Func<EventArgs,TReturn> action)
        {
            _globalEventDictionary[key] -= action;
        }

        public static TReturn Raise(string key)
        {
            if (!ContainsEvent(key)) return default;
            return _globalEventDictionary[key].Invoke(new EventArgs(){EventContext = null,EventName = key});
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
                _eventDictionary.Add(main,new Dictionary<string, Func<EventArgs,TReturn>>(){{key,null}});
                main.onDestroyContext += OnRequestRemoveData;
            }
        }

        private static void OnRequestRemoveData(IContext obj)
        {
            _eventDictionary[obj] = null;
            _eventDictionary.Remove(obj);
            obj.onDestroyContext -= OnRequestRemoveData;
        }

        public static void Register(IContext main, string key,Func<EventArgs,TReturn> action)
        {
            if(!ContainsEvent(main,key)) Install(main,key);
            _eventDictionary[main][key] += action;
        }

        public static void Unregister(IContext main, string key,Func<EventArgs,TReturn> action)
        {
            _eventDictionary[main][key] -= action;
        }

        public static TReturn Raise(IContext main, string key)
        {
            if (!ContainsEvent(main,key)) return default;
            return _eventDictionary[main][key].Invoke(new EventArgs(){EventContext = main,EventName = key});
        }
    
        public static TReturn TryRaise(IContext main, string key)
        {
            if (!ContainsEvent(main,key)) return default;
            return _eventDictionary[main][key].Invoke(new EventArgs(){EventContext = main,EventName = key});
        }

        public static bool ContainsEvent(IContext main, string key)
        {
            if (!_eventDictionary.ContainsKey(main)) return false;
            if (!_eventDictionary[main].ContainsKey(key)) return false;
            if (_eventDictionary[main][key] == null) return false;
            return true;
        }
    
        public static bool ContainsEvent(string key)
        {
            if (!_globalEventDictionary.ContainsKey(key)) return false;
            if (_globalEventDictionary[key] == null) return false;
            return true;
        }
    }
}
