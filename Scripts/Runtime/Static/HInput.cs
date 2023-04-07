using System;
using System.Collections.Generic;

namespace Dragon.Core
{
    public static class HInput
    {
        private static readonly Dictionary<string, Func<string,float>> _getAxisDict = new Dictionary<string, Func<string,float>>();
        private static readonly Dictionary<string, Func<string,bool>> _getButtonDict = new Dictionary<string, Func<string,bool>>();
        private static readonly Dictionary<string, Func<string,bool>> _getButtonDownDict = new Dictionary<string, Func<string,bool>>();
        private static readonly Dictionary<string, Func<string,bool>> _getButtonUpDict = new Dictionary<string, Func<string,bool>>();

        private static readonly Dictionary<string, Action<string>> _onButtonDown = new Dictionary<string, Action<string>>();
        private static readonly Dictionary<string, Action<string>> _onButtonUp = new Dictionary<string, Action<string>>();

        public static void DelegateGetButtonUp(string id, Func<string,bool> func)
        {
            _getButtonUpDict[id] = func;
        }
    
        public static void DelegateGetButtonDown(string id, Func<string,bool> func)
        {
            _getButtonDownDict[id] = func;
        }
    
        public static void DelegateGetButton(string id, Func<string,bool> func)
        {
            _getButtonDict[id] = func;
        }

        public static void DelegateGetAxis(string id, Func<string,float> func)
        {
            _getAxisDict[id] = func;
        }

        public static void RaiseButtonDown(string id)
        {
            if (!_onButtonDown.ContainsKey(id)) return;
            _onButtonDown[id].Invoke(id);
        }
    
        public static void RaiseButtonUp(string id)
        {
            if (!_onButtonUp.ContainsKey(id)) return;
            _onButtonUp[id].Invoke(id);
        }
    
        public static void RegisterOnButtonDown(string id, Action<string> action)
        {
            if (!_onButtonDown.ContainsKey(id))
            {
                _onButtonDown.Add(id,null);
            }
            _onButtonDown[id] += action;
        }

        public static void UnregisterOnButtonDown(string id, Action<string> action)
        {
            _onButtonDown[id] -= action;
        }

        public static void RegisterOnButtonUp(string id, Action<string> action)
        {
            if (!_onButtonUp.ContainsKey(id))
            {
                _onButtonUp.Add(id,null);
            }
            _onButtonUp[id] += action;
        }

        public static void UnregisterOnButtonUp(string id, Action<string> action)
        {
            _onButtonUp[id] -= action;
        }

        public static float GetAxis(string id)
        {
            return _getAxisDict[id].Invoke(id);
        }

        public static bool GetButton(string id)
        {
            return _getButtonDict[id].Invoke(id);
        }

        public static bool GetButtonDown(string id)
        {
            return _getButtonDownDict[id].Invoke(id);
        }

        public static bool GetButtonUp(string id)
        {
            return _getButtonUpDict[id].Invoke(id);
        }
    }
}