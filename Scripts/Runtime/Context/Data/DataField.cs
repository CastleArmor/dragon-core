using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace Dragon.Core
{
    [System.Serializable]
    [TopTitle(
        NameSuffix = "<color=#00ffff33><b>☵</b></color>",
        NamePrefix = "<color=#00ffff33><b>☵</b></color>",
        ShowGenericName = true,SetParentObject = true,
        BoldName = true,SetTransform = true,HideNameOnMid = true,ShowNameOnPrefix = true,ShowTypeOnSuffix = true)][GUIColor(0.5f,0.8f,1f)]
    public struct DataField<T>
    {
        [SerializeField]
        [HideInInspector] 
        private Transform _transform;
        public Transform transform
        {
            get => _transform;
            set
            {
                _transform = value;
            }
        }
    
        [SerializeField] [HideInInspector] private UnityEngine.Object _parentObject;
        public UnityEngine.Object parentObject
        {
            get => _parentObject;
            set => _parentObject = value;
        }

        public Type DataType => typeof(T);
    
        private bool _keyFieldToggled;
        [PropertyOrder(-1)]
        [HorizontalGroup(GroupID = "install",Width = 0.20f)]
        [Button("Single")][HideIf("HideIfShowKey")]
        private void ShowKey()
        {
            _keyFieldToggled = true;
        }

        private bool HideIfShowKey => _keyFieldToggled || Key != null;
        private bool ShowIfHideKey => _keyFieldToggled && Key == null;

        [PropertyOrder(-1)]
        [HorizontalGroup(GroupID = "install",Width = 0.05f)]
        [Button("X")][ShowIf("ShowIfHideKey")]
        private void HideKey()
        {
            _createBegun = false;
            _keyFieldToggled = false;
        }
    
        [HideInPlayMode]
        [SerializeField]
        [HideLabel]
        [ValueDropdown("GetAllAppropriateKeys")]
        [HorizontalGroup(GroupID = "install",Width = 0.40f)]
        [HideIf("HideIfKey")]
        public DataKey Key;
    
        [SerializeField]
        [InlineProperty]
        [HideLabel]
        [HorizontalGroup(GroupID = "install",Width = 0.55f)] 
        private AddressField _addressField;

        private bool HideIfKey => !_keyFieldToggled && Key == null;
    
        [ShowInInspector][ReadOnly][HideInEditorMode]
        private T _data;
        public T Data => _data;

        private IVar<T> _var;
        public IVar<T> Var => _var;

        private bool _createBegun;
        private bool ShouldShowCreateDataSetKey => Key == null;
        public bool CreateBegun => _createBegun && ShouldShowCreateDataSetKey;
        public bool ShowBeginCreate => !_createBegun && ShouldShowCreateDataSetKey && _keyFieldToggled && Key == null;
    
        [Button("Create")][HorizontalGroup(GroupID = "install",Width = 0.1f)][ShowIf("ShowBeginCreate")][PropertyOrder(-1)]
        public void BeginCreate()
        {
            _createBegun = true;
        }

        [ShowInInspector] [HideLabel] [HorizontalGroup(GroupID = "creation", Width = 0.8f)] [ShowIf("CreateBegun")]
        private string _keyName;

        [Button("Create")][HorizontalGroup(GroupID = "creation",Width = 0.1f)][ShowIf("CreateBegun")][PropertyOrder(-1)]
        public void CreateDataSetKey()
        {
            Key = DataKey.CreateAtFolder<T>(_keyName);
            _createBegun = false;
#if UNITY_EDITOR
            Undo.RecordObject(parentObject,"DictionaryKeyCreateAndSet");
            EditorUtility.SetDirty(parentObject);
#endif
        }
    
        private static IEnumerable GetAllAppropriateKeys()
        {
#if UNITY_EDITOR
            IEnumerable<DataKeyInfo> enumerable = GetAllResourceKeys();
            if (enumerable == null) return null;
            return enumerable
                .Where(EvaluateForAppropriateResource)
                .Select(PrepareDropdownItem);
#else
        return null;
#endif
        }

#if UNITY_EDITOR
        private static IEnumerable<DataKeyInfo> GetAllResourceKeys()
        {
            return DataKeyPath.GetKeys();
        }
#endif
        
        public void RegisterOnChange(IContext context, Action<DataOnChangeArgs<T>> action)
        {
            DataRegistry<T>.RegisterOnChange(_addressField.GetFromAddress(context), action, Key?Key.ID:"");
        }
    
        public void UnregisterOnChange(IContext context, Action<DataOnChangeArgs<T>> action)
        {
            DataRegistry<T>.UnregisterOnChange(_addressField.GetFromAddress(context), action, Key?Key.ID:"");
        }
    
        private static bool EvaluateForAppropriateResource(DataKeyInfo keyInfo)
        {
#if UNITY_EDITOR
            if (keyInfo.Key == null) return false;
            string[] types = keyInfo.Key.DataType.Split(';');
            foreach (var typestr in types)
            {
                System.Type type = System.Type.GetType(typestr);
                if (typestr == typeof(T).GetNiceName()) return true;
                else if (type != null)
                {
                    if (typeof(T).IsAssignableFrom(type))
                    {
                        return true;
                    }
                }
            }
            return false;
#else
        return false;
#endif
        }
    
        private static ValueDropdownItem PrepareDropdownItem(DataKeyInfo keyInfo)
        {
#if UNITY_EDITOR
            ValueDropdownItem item = new ValueDropdownItem();
            string path = keyInfo.Path.RemoveUntilString("DataKeys/");
            path = path.Replace("DataKeys/", "");
            item.Text = keyInfo.PathID+"/"+path.Replace(keyInfo.Key.name+".asset","")+keyInfo.Key.ID;
            item.Value = keyInfo.Key;
            return item;
#else
        return default;
#endif
        }

        public bool TryGet(IContext context)
        {
            string key = Key ? Key.ID : "";
            if (DataRegistry<T>.ContainsData(_addressField.GetFromAddress(context), key))
            {
                _data = DataRegistry<T>.GetData(_addressField.GetFromAddress(context),key);
                return true;
            }

            return false;
        }
    
        public T Get(IContext context)
        {
            string key = Key ? Key.ID : "";
            _data = DataRegistry<T>.GetData(_addressField.GetFromAddress(context),key);
            return _data;
        }

        /// <summary>
        /// Beware if non existent, this will create a variable for a data.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IVar<T> GetVar(IContext context)
        {
            string key = Key ? Key.ID : "";
            _var = DataRegistry<T>.GetVariable(_addressField.GetFromAddress(context), key);
            return _var;
        }

        public void Set(IContext context, T value)
        {
            string key = Key ? Key.ID : "";
            DataRegistry<T>.SetData(_addressField.GetFromAddress(context),value,key);
        }

        public static DataField<T> SingleContextRoot()
        {
            return new DataField<T>()
            {
                _addressField = AddressField.SingleContextRoot()
            };
        }
    }
}