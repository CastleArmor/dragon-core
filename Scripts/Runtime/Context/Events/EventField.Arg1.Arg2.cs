using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

#region TArg1,TArg2

namespace Dragon.Core
{
    [System.Serializable][TopTitle(ShowGenericName = true,
        NameSuffix = "<color=#ff77ff55><b>↖EVENT</b></color>",
        NamePrefix = "<color=#ff77ff55><b>↖</b></color>",
        PerGenericArgString = ",",
        BoldName = true)][GUIColor(1f,0.6f,1f)]
    public struct EventField<TArg1,TArg2>:IEventInstaller,ISerializationCallbackReceiver
    {
        [HideInPlayMode][SerializeField][HideLabel][HorizontalGroup(GroupID = "install",Width = 0.2f)] private EventAddressType _addressType;

        [ValueDropdown("GetAllAppropriateKeys")][ValidateInput("ValidateCurrentKey")][OnValueChanged("OnEventKeyChanged")]
        [HideInPlayMode][SerializeField][HideLabel][HorizontalGroup(GroupID = "install",Width = 0.8f)] private EventKey _eventKey;
        
        private void OnEventKeyChanged()
        {
            if (_eventKey == null) return;
            if (_eventKey.MustBeGlobal)
            {
                _addressType = EventAddressType.Global;
            }
        }
        
        [SerializeField] [HideInInspector] private UnityEngine.Object _parentObject;

        public UnityEngine.Object parentObject
        {
            get => _parentObject;
            set => _parentObject = value;
        }

        #region KeyCreation

        [ShowInInspector][HideLabel][ShowIf("ShowCreationOptions")][HorizontalGroup(GroupID = "creation")]
        private string _keyName;

        [ShowInInspector][HideLabel][SuffixLabel("Should Be Global")][ShowIf("ShowCreationOptions")][HorizontalGroup(GroupID = "creation",MaxWidth = 0.15f)]
        private bool _createdKeyShouldBeGlobal;
        private bool ShowStartCreatingKey => _eventKey == null && !_isCreatingStarted;
        private bool _isCreatingStarted;
        private bool ShowCreationOptions => _eventKey == null && _isCreatingStarted;
#if UNITY_EDITOR
        [Button("Create")][HorizontalGroup(GroupID = "install")][ShowIf("ShowStartCreatingKey")]
        private void StartCreatingKey()
        {
            _isCreatingStarted = true;
        }
        [Button("Create")][ShowIf("ShowCreationOptions")][HorizontalGroup(GroupID = "creation")]
        private void CreateKey()
        {
            ScriptableObject obj = ScriptableObject.CreateInstance(typeof(EventKey));
            EventKey eventKey = obj as EventKey;
            eventKey.SetupKey(_keyName,_createdKeyShouldBeGlobal,typeof(TArg1).GetNiceName(),typeof(TArg2).GetNiceName());
            EnsurePathExistence("Assets/_Project/ScriptAssets/EventKeys");
            string uniquePath = AssetDatabase.GenerateUniqueAssetPath("Assets/_Project/ScriptAssets/EventKeys" + "/" + eventKey.name + ".asset");
            AssetDatabase.CreateAsset(eventKey,uniquePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            GetAllResourceKeys();
            Undo.RecordObject(parentObject,"CreatedKeyAndSet");
            _eventKey = eventKey;
            _isCreatingStarted = false;
            EditorUtility.SetDirty(parentObject);
        }
        [Button("Cancel")][ShowIf("ShowCreationOptions")][HorizontalGroup(GroupID = "creation")]
        private void CancelCreate()
        {
            _isCreatingStarted = false;
        }
        /// <summary>
        /// If directory path doesn't exist it will create, if it exist it will do nothing.
        /// </summary>
        /// <param name="path"></param>
        public void EnsurePathExistence(string path)
        {
            string[] splitFoldersArray = path.Split('/');
            List<string> splitFolders = splitFoldersArray.ToList();
            splitFolders.RemoveAt(0); //Removing Assets directory it's special.

            //Ensure path exists.
            string directory = "Assets";
            foreach (string folder in splitFolders)
            {
                if (!AssetDatabase.IsValidFolder(directory+"/"+folder))
                    AssetDatabase.CreateFolder(directory, folder);

                directory += "/" + folder;
            }
        }
#endif
        #endregion

#if UNITY_EDITOR
        private static IEnumerable<EventKeyInfo> GetAllResourceKeys()
        {
            return EventKeyPath.GetKeys();
        }
#endif

        private bool ValidateCurrentKey()
        {
            if(_eventKey == null) return true;
            OnEventKeyChanged();
            return _eventKey.Arg1Type == typeof(TArg1).GetNiceName() && _eventKey.Arg2Type == typeof(TArg2).GetNiceName() && string.IsNullOrEmpty(_eventKey.ReturnType);
        }

        private static bool EvaluateForAppropriateResource(EventKeyInfo keyInfo)
        {
#if UNITY_EDITOR
            if (keyInfo.Key == null) return false;
            if (keyInfo.Key is EventKey eventKey)
            {
                return eventKey.Arg1Type == typeof(TArg1).GetNiceName() && eventKey.Arg2Type == typeof(TArg2).GetNiceName() && string.IsNullOrEmpty(eventKey.ReturnType);
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
    
        public bool FromRelative => _addressType == EventAddressType.ContextRelative;
        public bool FromUserRelative => _addressType == EventAddressType.ContextUserRelative;

        public bool ShowRelationStack => FromUserRelative || FromRelative;
        
        public void Install(IEventContext selfMain)
        {
            InstallForEach(selfMain);
        }

        public void Remove()
        {
            RemoveForEach();
        }

        public void Register(IEventContext selfMain,Action<EventArgs,TArg1,TArg2> action)
        {
            if (_eventKey == null) return;
            RegisterForEach(selfMain,action);
        }

        public void Unregister(IEventContext selfMain, Action<EventArgs,TArg1,TArg2> action)
        {
            if (_eventKey == null) return;
            UnregisterForEach(selfMain,action);
        }

        [Button][HideInEditorMode]
        public void Raise(IEventContext selfMain,TArg1 arg1,TArg2 arg2)
        {
            RaiseForEach(selfMain,arg1,arg2);
        }

        #region FOREACH_FUNCTIONS
        
        private void RemoveForEach()
        {
            if (_eventKey == null) return;
            if ((_addressType & EventAddressType.Global) != 0)
            {
                EventRegistry<TArg1,TArg2>.Remove(_eventKey.ID);
            }
        }
        
        private void InstallForEach(IEventContext selfMain)
        {
            if (_eventKey == null) return;
            if (_addressType == EventAddressType.OnlyContext)
            {
                EventRegistry<TArg1,TArg2>.Install(selfMain, _eventKey.ID);
                return;
            }
            if ((_addressType & EventAddressType.Global) != 0)
            {
                EventRegistry<TArg1,TArg2>.Install(_eventKey.ID);
            }
            if ((_addressType & EventAddressType.Context) != 0)
            {
                EventRegistry<TArg1,TArg2>.Install(selfMain, _eventKey.ID);
            }
        }

        private void RegisterForEach(IEventContext selfMain, Action<EventArgs,TArg1,TArg2> action)
        {
            if (_eventKey == null) return;
            if (_addressType == EventAddressType.OnlyContext)
            {
                EventRegistry<TArg1,TArg2>.Register(selfMain, _eventKey.ID, action);
                return;
            }
            if ((_addressType & EventAddressType.Global) != 0)
            {
                EventRegistry<TArg1,TArg2>.Register(_eventKey.ID, action);
            }
            if ((_addressType & EventAddressType.Context) != 0)
            {
                EventRegistry<TArg1,TArg2>.Register(selfMain, _eventKey.ID, action);
            }
        }

        private void UnregisterForEach(IEventContext selfMain, Action<EventArgs,TArg1,TArg2> action)
        {
            if (_eventKey == null) return;
            if (_addressType == EventAddressType.OnlyContext)
            {
                EventRegistry<TArg1,TArg2>.Unregister(selfMain, _eventKey.ID, action);
                return;
            }
            if ((_addressType & EventAddressType.Global) != 0)
            {
                EventRegistry<TArg1,TArg2>.Unregister(_eventKey.ID, action);
            }
            if ((_addressType & EventAddressType.Context) != 0)
            {
                EventRegistry<TArg1,TArg2>.Unregister(selfMain, _eventKey.ID, action);
            }
        }
        
        private void RaiseForEach(IEventContext selfMain,TArg1 arg1,TArg2 arg2)
        {
            if (_addressType == EventAddressType.OnlyContext)
            {
                EventRegistry<TArg1,TArg2>.Raise(selfMain, _eventKey.ID, arg1,arg2);
                return;
            }
            if ((_addressType & EventAddressType.Global) != 0)
            {
                EventRegistry<TArg1,TArg2>.Raise(_eventKey.ID, arg1,arg2);
            }
            if ((_addressType & EventAddressType.Context) != 0)
            {
                EventRegistry<TArg1,TArg2>.Raise(selfMain, _eventKey.ID, arg1,arg2);
            }
        }

        #endregion

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
    
    #endregion