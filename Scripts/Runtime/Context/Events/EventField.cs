﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Dragon.Core
{
    [System.Serializable]
    [TopTitle(ShowGenericName = false,
        NameSuffix = "<color=#ff77ff55><b>↖EVENT</b></color>",
        NamePrefix = "<color=#ff77ff55><b>↖</b></color>",
        PerGenericArgString = ",",SetParentObject = true,
        BoldName = true)][GUIColor(1f,0.6f,1f)]
    public struct EventField:IEventInstaller,ISerializationCallbackReceiver
    { 
        [SerializeField] [HideInInspector] private UnityEngine.Object _parentObject;

        public UnityEngine.Object parentObject
        {
            get => _parentObject;
            set => _parentObject = value;
        }
        [DisableInPlayMode][SerializeField][HideLabel][HorizontalGroup(GroupID = "install",Width = 0.2f)] private EventAddressType _addressType;

        [SerializeField][HideIf("HasEventKey")][HorizontalGroup(GroupID = "install")][LabelWidth(80f)] private bool _ignoreIfNull;

        [ValueDropdown("GetAllAppropriateKeys")][ValidateInput("ValidateCurrentKey")][OnValueChanged("OnEventKeyChanged")]
        [DisableInPlayMode][SerializeField][HideLabel][HorizontalGroup(GroupID = "install")][HideIf("IsKeyIgnored")] private EventKey _eventKey;

        [SerializeField][ShowIf("IsFromGroupFirstMember")]
        private Key _groupKey;

        public bool HasEventKey => _eventKey != null;

        public bool IsKeyIgnored => _ignoreIfNull && _eventKey == null;

        [Button("*")][HorizontalGroup(GroupID = "install",Width = 0.01f)]
        private void UpdateButton()
        {
            EventKeyPath.RequestUpdate();
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
        [Button("Create")][HorizontalGroup(GroupID = "install",Width = 0.2f)][ShowIf("ShowStartCreatingKey")]
        private void StartCreatingKey()
        {
            _isCreatingStarted = true;
        }
        [Button("Create")][ShowIf("ShowCreationOptions")][HorizontalGroup(GroupID = "creation")]
        private void CreateKey()
        {
            ScriptableObject obj = ScriptableObject.CreateInstance(typeof(EventKey));
            EventKey eventKey = obj as EventKey;
            eventKey.SetupKey(_keyName,_createdKeyShouldBeGlobal);
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

        private void OnEventKeyChanged()
        {
            if (_eventKey == null) return;
            if (_eventKey.MustBeGlobal)
            {
                _addressType = EventAddressType.Global;
            }
        }
        
        private bool ValidateCurrentKey()
        {
            if (_eventKey == null) return true;
            if (_eventKey.MustBeGlobal)
            {
                _addressType = EventAddressType.Global;
            }
            return string.IsNullOrEmpty(_eventKey.Arg1Type) && string.IsNullOrEmpty(_eventKey.Arg2Type) && string.IsNullOrEmpty(_eventKey.ReturnType);
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
                return string.IsNullOrEmpty(eventKey.Arg1Type) && string.IsNullOrEmpty(eventKey.Arg2Type) && string.IsNullOrEmpty(eventKey.ReturnType);
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
        public bool IsFromGroupFirstMember => _addressType == EventAddressType.FromGroupFirstMember;

        public bool ShowRelationStack => FromUserRelative || FromRelative;

        public void Install(IContext selfMain)
        {
            InstallForEach(selfMain);
        }

        public void Remove()
        {
            RemoveForEach();
        }

        public void Register(IContext selfMain,Action<EventArgs> action)
        {
            if (_eventKey == null) return;
            RegisterForEach(selfMain,action);
        }

        public void Unregister(IContext selfMain, Action<EventArgs> action)
        {
            if (_eventKey == null) return;
            UnregisterForEach(selfMain,action);
        }

        [Button][HideInEditorMode]
        public void Raise(IContext selfMain)
        {
            RaiseForEach(selfMain);
        }
        
        private void RemoveForEach()
        {
            if (_eventKey == null) return;
            if ((_addressType & EventAddressType.Global) != 0)
            {
                EventRegistry.Remove(_eventKey.ID);
            }
        }

        private void InstallForEach(IContext selfMain)
        {
            
            if (_addressType == EventAddressType.OnlyContext)
            {
                EventRegistry.Install(selfMain, _eventKey.ID);
                return;
            }
            if ((_addressType & EventAddressType.Global) != 0)
            {
                EventRegistry.Install(_eventKey.ID);
            }
            if ((_addressType & EventAddressType.Context) != 0)
            {
                EventRegistry.Install(selfMain, _eventKey.ID);
            }
            if ((_addressType & EventAddressType.FromGroupFirstMember) != 0)
            {
                EventRegistry.Install(DataRegistry<List<IActor>>.GetData(null,_groupKey.ID)[0].pContext, _eventKey.ID);
            }
        }
        
        private Action<EventArgs> RegisterForEach(IContext selfMain, Action<EventArgs> action)
        {
            if (_eventKey == null) return null;
            if (_addressType == EventAddressType.OnlyContext)
            {
                return EventRegistry.Register(selfMain, _eventKey.ID, action);
            }

            Action<EventArgs> returned = null;
            if ((_addressType & EventAddressType.Global) != 0)
            {
                returned = EventRegistry.Register(_eventKey.ID, action);
            }
            if ((_addressType & EventAddressType.Context) != 0)
            {
                returned = EventRegistry.Register(selfMain, _eventKey.ID, action);
            }
            if ((_addressType & EventAddressType.FinalUser) != 0)
            {
                returned = EventRegistry.Register(selfMain.RootContext, _eventKey.ID,action);
            }
            if ((_addressType & EventAddressType.FromGroupFirstMember) != 0)
            {
                returned = EventRegistry.Register(DataRegistry<List<IActor>>.GetData(null,_groupKey.ID)[0].pContext, _eventKey.ID, action);
            }

            return returned;
        }

        private  Action<EventArgs> UnregisterForEach(IContext selfMain, Action<EventArgs> action)
        {
            if (_eventKey == null) return null;
            if (_addressType == EventAddressType.OnlyContext)
            {
                return EventRegistry.Unregister(selfMain, _eventKey.ID, action);
            }
            Action<EventArgs> returned = null;
            if ((_addressType & EventAddressType.Global) != 0)
            {
                returned = EventRegistry.Unregister(_eventKey.ID, action);
            }
            if ((_addressType & EventAddressType.Context) != 0)
            {
                returned = EventRegistry.Unregister(selfMain, _eventKey.ID, action);
            }
            if ((_addressType & EventAddressType.FinalUser) != 0)
            {
                returned = EventRegistry.Unregister(selfMain.RootContext, _eventKey.ID,action);
            }
            if ((_addressType & EventAddressType.FromGroupFirstMember) != 0)
            {
                returned = EventRegistry.Unregister(DataRegistry<List<IActor>>.GetData(null,_groupKey.ID)[0].pContext, _eventKey.ID, action);
            }

            return returned;
        }
        
        private void RaiseForEach(IContext selfMain)
        {
            if (_ignoreIfNull)
            {
                if (_eventKey == null) return;
            }
            if (_addressType == EventAddressType.OnlyContext)
            {
                EventRegistry.Raise(selfMain, _eventKey.ID);
                return;
            }
            if ((_addressType & EventAddressType.Global) != 0)
            {
                EventRegistry.Raise(_eventKey.ID);
            }
            if ((_addressType & EventAddressType.Context) != 0)
            {
                EventRegistry.Raise(selfMain, _eventKey.ID);
            }
            if ((_addressType & EventAddressType.FinalUser) != 0)
            {
                EventRegistry.Raise(selfMain.RootContext, _eventKey.ID);
            }
            if ((_addressType & EventAddressType.FromGroupFirstMember) != 0)
            {
                EventRegistry.Raise(DataRegistry<List<IActor>>.GetData(null,_groupKey.ID)[0].pContext, _eventKey.ID);
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