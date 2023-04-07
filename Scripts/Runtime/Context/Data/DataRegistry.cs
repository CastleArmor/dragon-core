using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Dragon.Core
{
    public static class DataRegistry<T>
    {
        private static readonly Dictionary<string, T> _allData = new Dictionary<string, T>();
        public static Dictionary<string, T> AllData => _allData;
        
        private static readonly Dictionary<string, T> _installedData = new Dictionary<string, T>();
        public static Dictionary<string, T> InstalledData => _installedData;

        private static readonly Dictionary<string, List<string>> _contextKeys =
            new Dictionary<string, List<string>>();

        private static readonly Dictionary<string, Action<DataOnChangeArgs<T>>> _onChangeDictionary =
            new Dictionary<string, Action<DataOnChangeArgs<T>>>();

        private static readonly Dictionary<string, IVar<T>> _variables = new Dictionary<string, IVar<T>>();

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
                if (!AllData.ContainsKey(key)) continue;
                if (AllData[key] is IContextData installedData)
                {
                    installedData.OnRemoveData();
                }
                AllData.Remove(key);
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
                if (AllData[key] is ISaveableData saveable)
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

        public static void RegisterVariable(IContext context, string key, IVar<T> variable)
        {
            string assignedID = GetAssignedKey(context, key);
            _variables.Add(assignedID,variable);
        }

        public static void UnregisterVariable(IContext context, string key, IVar<T> variable)
        {
            string assignedID = GetAssignedKey(context, key);
            _variables.Remove(assignedID);
        }

        public static void BindData(IContext context,T value, string key)
        {
            EnsureEventRegisters();

            string assignedID = GetAssignedKey(context, key);
            if (context != null)
            {
                context.onDestroyContext += (c) => UnbindData(c,key);
            }

            AllData[assignedID] = value;
        }

        public static void UnbindData(IContext context,string key)
        {
            EnsureEventRegisters();
        
            string assignedID = GetAssignedKey(context, key);
            if (context != null)
            {
                context.onDestroyContext -= (c) => UnbindData(c,key);
            }
        
            AllData.Remove(assignedID);
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
            if (!InstalledData.ContainsKey(assignedID))
            {
                return;
            }
            T oldValue = InstalledData[assignedID];
            if (oldValue is IAdditionalDataBinder binder)
            {
                binder.OnToggleBinding(context,key,assignedID,false);
            }
            if (oldValue is IInitializable initializable)
            {
                initializable.FinalizeIfNot();
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
            if (InstalledData.ContainsKey(assignedID))
            {
                oldValue = InstalledData[assignedID];
            }
            InstalledData[assignedID] = value;
            AllData[assignedID] = value;
            if (value != null)
            {
                if (value is IInitializable initializable)
                {
                    if (initializable.IsInitialized)
                    {
                        Debug.LogError("This is already initialized and installed data context = " + (context!=null?context.name:"Global") + ", key = " + key + " type = " + typeof(T));
                    }
                }
                
                if (value is IContextInstallable installable)
                {
                    installable.SetInstallParameters(context,key,assignedID);
                }

                if (value is IAdditionalDataBinder binder)
                {
                    binder.OnToggleBinding();
                }
            }

            if (_onChangeDictionary.ContainsKey(assignedID))
            {
                if (!_equalityComparer.Equals(oldValue, value))
                {
                    _onChangeDictionary[assignedID]?.Invoke(new DataOnChangeArgs<T>()
                    {
                        AssignedKey = assignedID,
                        Context = context,
                        OldValue = oldValue,
                        NewValue = value
                    });
                }
            }
            
            if (oldValue != null)
            {
                if (oldValue is IInitializable initializable)
                {
                    initializable.FinalizeIfNot();
                }
            }
        }

        public static bool ContainsData(string fullKey)
        {
            return AllData.ContainsKey(fullKey);
        }

        public static bool ContainsData(IContext context, string key)
        {
            string assignedID = GetAssignedKey(context, key);
            return AllData.ContainsKey(assignedID);
        }
    
        public static bool ContainsData(string contextID, string key)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append(contextID);
            _stringBuilder.Append("/");
            _stringBuilder.Append(key);
            string assignedID = _stringBuilder.ToString();
            return AllData.ContainsKey(assignedID);
        }

        public static IVar<T> GetVariable(IContext context, string key = "")
        {
            if (ContainsData(context, key))
            {
                string assignedKey = GetAssignedKey(context, key);
                if (_variables.ContainsKey(assignedKey))
                {
                    return _variables[assignedKey];
                }
                else
                {
                    IVar<T> newVariable = new Var<T>();
                    newVariable.InitializeVariable(context as IContext, key);
                    return newVariable;
                }
            }
            else
            {
                Debug.LogError("Trying to get variable of non existent data, " + context.ContextID + " - Key=" + key);
                return null;
            }
        }

        public static bool TryGetData(IContext context, out T data, string key = "")
        {
            if (ContainsData(context, key))
            {
                data = GetData(context, key);
                return true;
            }

            data = default;
            return false;
        }

        public static void TryActionOnData(IContext context, Action<T> action, string key = "")
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
            if (!AllData.ContainsKey(assignedID))
            {
                Debug.LogError(context.As<IUnityObject>().name + " doesn't contain " + typeof(T).Name + " With " + (string.IsNullOrEmpty(key)?"Single":key));
            }
#endif
            return AllData[assignedID];
        }
    
        public static T GetDataByID(string contextID, string key = "")
        {
            string assignedID = contextID+"/"+key;
            return AllData[assignedID];
        }

        private static void OnDestroyContextOfInstalledData(IContext context)
        {
            context.onDestroyContext -= OnDestroyContextOfInstalledData;
            foreach (string dataKey in _contextKeys[ContextRegistry.GetID(context)])
            {
                if (!AllData.ContainsKey(dataKey)) continue;
                if (AllData[dataKey] is IInitializable initializable)
                {
                    initializable.FinalizeIfNot();
                }
                AllData.Remove(dataKey);
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
}