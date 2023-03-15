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
    public struct ReturnEventField<TArg1,TReturn>:IEventInstaller,ISerializationCallbackReceiver
    {
        [DisableInPlayMode][SerializeField][HideLabel][HorizontalGroup(GroupID = "install",Width = 0.2f)] private ReturnEventAddressType _addressType;

        [ValueDropdown("GetAllAppropriateKeys")][OnValueChanged("OnEventKeyChanged")][ValidateInput("ValidateCurrentKey")]
        [DisableInPlayMode][SerializeField][HideLabel][HorizontalGroup(GroupID = "install",Width = 0.8f)] private EventKey _eventKey;
        public EventKey EventKey =>_eventKey;

        private bool ShowDefaultReturnValue => typeof(bool) == typeof(TReturn) && _eventKey == null; 
    
        [SerializeField][ShowIf("ShowDefaultReturnValue")] private TReturn _defaultReturnValue;

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
            return _eventKey.Arg1Type == typeof(TArg1).GetNiceName() && string.IsNullOrEmpty(_eventKey.Arg2Type) && _eventKey.ReturnType == typeof(TReturn).GetNiceName();
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
                return eventKey.Arg1Type == typeof(TArg1).GetNiceName() && string.IsNullOrEmpty(eventKey.Arg2Type) && eventKey.ReturnType == typeof(TReturn).GetNiceName();
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
    
        public void Install(IEventContext selfMain)
        {
            if (_addressType == ReturnEventAddressType.Global)
            {
                ReturnEventRegistry<TArg1,TReturn>.Install(_eventKey.ID);
            }
            else
            {
                ReturnEventRegistry<TArg1,TReturn>.Install(GetAddressMain(selfMain), _eventKey.ID);
            }
        }

        public void Remove()
        {
            if (_addressType == ReturnEventAddressType.Global)
            {
                ReturnEventRegistry<TArg1,TReturn>.Remove(_eventKey.ID);
            }
        }

        //Optional
        public void Register(IEventContext selfMain,Func<TArg1,TReturn> action)
        {
            if (_eventKey == null) return;
            if (_addressType == ReturnEventAddressType.Global)
            {
                ReturnEventRegistry<TArg1,TReturn>.Register(_eventKey.ID, action);
            }
            else
            {
                ReturnEventRegistry<TArg1,TReturn>.Register(GetAddressMain(selfMain), _eventKey.ID, action);
            }
        }

        //Optional
        public void Unregister(IEventContext selfMain, Func<TArg1,TReturn> action)
        {
            if (_eventKey == null) return;
            if (_addressType == ReturnEventAddressType.Global)
            {
                ReturnEventRegistry<TArg1,TReturn>.Unregister(_eventKey.ID, action);
            }
            else
            {
                ReturnEventRegistry<TArg1,TReturn>.Unregister(GetAddressMain(selfMain), _eventKey.ID, action);
            }
        }
    
        //MUST!
        [Button][HideInEditorMode]
        public TReturn Raise(IEventContext selfMain,TArg1 arg1)
        {
            if (_eventKey == null)
            {
                return _defaultReturnValue;
            }
        
            if (_addressType == ReturnEventAddressType.Global)
            {
                _lastReturnedValue = ReturnEventRegistry<TArg1,TReturn>.Raise(_eventKey.ID,arg1);
                return _lastReturnedValue;
            }
            else
            {
                _lastReturnedValue = ReturnEventRegistry<TArg1,TReturn>.Raise(GetAddressMain(selfMain), _eventKey.ID,arg1);
                return _lastReturnedValue;
            }
        }

        private IEventContext GetAddressMain(IEventContext selfMain)
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
