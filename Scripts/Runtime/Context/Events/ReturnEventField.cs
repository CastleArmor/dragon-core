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
        PerGenericArgString = ",",BoldName = true,
        LastIsReturn = true)][GUIColor(1f,0.6f,1f)]
    public struct ReturnEventField<TReturn>:IEventInstaller,ISerializationCallbackReceiver
    {
        [DisableInPlayMode][SerializeField][HideLabel][HorizontalGroup(GroupID = "install",Width = 0.2f)] private ReturnEventAddressType _addressType;

        [ValueDropdown("GetAllAppropriateKeys")][OnValueChanged("OnEventKeyChanged")][ValidateInput("ValidateCurrentKey")]
        [DisableInPlayMode][SerializeField][HideLabel][HorizontalGroup(GroupID = "install",Width = 0.8f)] private EventKey _eventKey;
    
        private bool ShowDefaultReturnValue => typeof(bool) == typeof(TReturn) && _eventKey == null;  
    
        [SerializeField][ShowIf("ShowDefaultReturnValue")] private TReturn _defaultReturnValue;

        [HideInEditorMode]
        [ShowInInspector] [ReadOnly] private TReturn _lastReturnedValue;

        public TReturn LastReturnedValue => _lastReturnedValue;
    
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
            if (_eventKey.MustBeGlobal)
            {
                _addressType = ReturnEventAddressType.Global;
            }
            return string.IsNullOrEmpty(_eventKey.Arg1Type) && string.IsNullOrEmpty(_eventKey.Arg2Type) && _eventKey.ReturnType == typeof(TReturn).GetNiceName();
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
                return string.IsNullOrEmpty(eventKey.Arg1Type) && string.IsNullOrEmpty(eventKey.Arg2Type) && eventKey.ReturnType == typeof(TReturn).GetNiceName();
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
                ReturnEventRegistry<TReturn>.Install(_eventKey.ID);
            }
            else
            {
                ReturnEventRegistry<TReturn>.Install(GetAddressMain(selfMain), _eventKey.ID);
            }
        }

        public void Remove()
        {
            if (_addressType == ReturnEventAddressType.Global)
            {
                ReturnEventRegistry<TReturn>.Remove(_eventKey.ID);
            }
        }

        public void Register(IEventContext selfMain,Func<EventArgs,TReturn> action)
        {
            if (_eventKey == null) return;
            if (_addressType == ReturnEventAddressType.Global)
            {
                ReturnEventRegistry<TReturn>.Register(_eventKey.ID, action);
            }
            else
            {
                ReturnEventRegistry<TReturn>.Register(GetAddressMain(selfMain), _eventKey.ID, action);
            }
        }

        public void Unregister(IEventContext selfMain, Func<EventArgs,TReturn> action)
        {
            if (_eventKey == null) return;
            if (_addressType == ReturnEventAddressType.Global)
            {
                ReturnEventRegistry<TReturn>.Unregister(_eventKey.ID, action);
            }
            else
            {
                ReturnEventRegistry<TReturn>.Unregister(GetAddressMain(selfMain), _eventKey.ID, action);
            }
        }

        [Button][HideInEditorMode]
        public TReturn Raise(IEventContext selfMain)
        {
            if (_eventKey == null)
            {
                return _defaultReturnValue;
            }
            if (_addressType == ReturnEventAddressType.Global)
            {
                _lastReturnedValue = ReturnEventRegistry<TReturn>.Raise(_eventKey.ID);
                return _lastReturnedValue;
            }
            else
            {
                _lastReturnedValue = ReturnEventRegistry<TReturn>.Raise(GetAddressMain(selfMain), _eventKey.ID);
                return _lastReturnedValue;
            }
        }
        private IEventContext GetAddressMain(IEventContext selfMain)
        {
            return DEventConstructs.GetReturnEventAddressMain(selfMain,_addressType).As<IEventContext>();
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