using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace Dragon.Core
{
    [System.Serializable][TopTitle(ShowGenericName = true,
        NameSuffix = "<color=#ff77ff55><b>☇EVENT</b></color>",
        NamePrefix = "<color=#ff77ff55><b>☇</b></color>",
        PerGenericArgString = ",",
        BoldName = true,
        LastIsReturn = true)][GUIColor(1f,0.6f,1f)]
    public struct 
        ReturnEventField<TArg1,TArg2,TReturn>:IEventInstaller,ISerializationCallbackReceiver
    {
        [DisableInPlayMode][SerializeField][HideLabel][HorizontalGroup(GroupID = "install",Width = 0.2f)] private ReturnEventAddressType _addressType;

        [ValueDropdown("GetAllAppropriateKeys")][OnValueChanged("OnEventKeyChanged")][ValidateInput("ValidateCurrentKey")]
        [DisableInPlayMode][SerializeField][HideLabel][HorizontalGroup(GroupID = "install",Width = 0.8f)] private EventKey _eventKey;
    
        public bool HasEventKey => _eventKey != null;
    
        [HideInEditorMode]
        [ShowInInspector] [ReadOnly] private TReturn _lastReturnedValue;
    
        private void OnEventKeyChanged()
        {
            if (_eventKey == null) return;
            if (_eventKey.MustBeGlobal)
            {
                _addressType = ReturnEventAddressType.Global;
            }
        }
    
        private bool ValidateCurrentKey()
        {
            if (_eventKey == null) return true;
            OnEventKeyChanged();
            return _eventKey.Arg1Type == typeof(TArg1).GetNiceName() && _eventKey.Arg2Type == typeof(TArg2).GetNiceName() && _eventKey.ReturnType == typeof(TReturn).GetNiceName();
        }

#if UNITY_EDITOR
        private static IEnumerable<EventKeyInfo> GetAllResourceKeys()
        {
            return EventKeyPath.GetKeys();
        }
#endif

        private static bool EvaluateForAppropriateResource(EventKeyInfo keyInfo)
        {
#if UNITY_EDITOR
            if (keyInfo.Key == null) return false;
            if (keyInfo.Key is EventKey eventKey)
            {
                return eventKey.Arg1Type == typeof(TArg1).GetNiceName() && eventKey.Arg2Type == typeof(TArg2).GetNiceName() && eventKey.ReturnType == typeof(TReturn).GetNiceName();
            }

            return false;
#else
    return false;
#endif
        }

        private static ValueDropdownItem PrepareDropdownItem(EventKeyInfo keyInfo)
        {
#if UNITY_EDITOR
            ValueDropdownItem item = new ValueDropdownItem();
            if (keyInfo.Key is EventKey eventKey)
            {
                string path = keyInfo.Path.RemoveUntilString("EventKeys/");
                path = path.Replace("EventKeys/", "");
                item.Text = keyInfo.PathID+"/"+path.Replace(eventKey.name+".asset","")+eventKey.ID;
                item.Value = keyInfo.Key;
                return item;
            }

            return default;
#else
    return default;
#endif
        }

        private static IEnumerable GetAllAppropriateKeys()
        {
#if UNITY_EDITOR
            return GetAllResourceKeys()
                .Where(EvaluateForAppropriateResource)
                .Select(PrepareDropdownItem);
#else
    return null;
#endif
        }

        public bool FromRelative => _addressType == ReturnEventAddressType.ContextRelative;
        public bool FromUserRelative => _addressType == ReturnEventAddressType.ContextUserRelative;

        public bool ShowRelationStack => FromUserRelative || FromRelative;
    
        public void Install(IContext selfMain)
        {
            if (_addressType == ReturnEventAddressType.Global)
            {
                ReturnEventRegistry<TArg1,TArg2,TReturn>.Install(_eventKey.ID);
            }
            else
            {
                ReturnEventRegistry<TArg1,TArg2,TReturn>.Install(GetAddressMain(selfMain), _eventKey.ID);
            }
        }

        public void Remove()
        {
            if (_addressType == ReturnEventAddressType.Global)
            {
                ReturnEventRegistry<TArg1,TArg2,TReturn>.Remove(_eventKey.ID);
            }
        }

        public void Register(IContext selfMain,Func<TArg1,TArg2,TReturn> action)
        {
            if (_eventKey == null) return;
            if (_addressType == ReturnEventAddressType.Global)
            {
                ReturnEventRegistry<TArg1,TArg2,TReturn>.Register(_eventKey.ID, action);
            }
            else
            {
                ReturnEventRegistry<TArg1,TArg2,TReturn>.Register(GetAddressMain(selfMain), _eventKey.ID, action);
            }
        }

        public void Unregister(IContext selfMain, Func<TArg1,TArg2,TReturn> action)
        {
            if (_eventKey == null) return;
            if (_addressType == ReturnEventAddressType.Global)
            {
                ReturnEventRegistry<TArg1,TArg2,TReturn>.Unregister(_eventKey.ID, action);
            }
            else
            {
                ReturnEventRegistry<TArg1,TArg2,TReturn>.Unregister(GetAddressMain(selfMain), _eventKey.ID, action);
            }
        }

        [Button][HideInEditorMode]
        public TReturn Raise(IContext selfMain,TArg1 arg1,TArg2 arg2)
        {
            if (_addressType == ReturnEventAddressType.Global)
            {
                _lastReturnedValue = ReturnEventRegistry<TArg1,TArg2,TReturn>.Raise(_eventKey.ID,arg1,arg2);
                return _lastReturnedValue;
            }
            else
            {
                _lastReturnedValue = ReturnEventRegistry<TArg1,TArg2,TReturn>.Raise(GetAddressMain(selfMain), _eventKey.ID,arg1,arg2);
                return _lastReturnedValue;
            }
        }

        private IContext GetAddressMain(IContext selfMain)
        {
            switch (_addressType)
            {
                case ReturnEventAddressType.Context : 
                    return selfMain;
                default: return selfMain;
            }
        }

        public void OnBeforeSerialize()
        {
            OnEventKeyChanged();
        }

        public void OnAfterDeserialize()
        {
            OnEventKeyChanged();
        }
    }
}
