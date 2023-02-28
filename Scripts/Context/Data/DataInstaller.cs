using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[System.Serializable][GUIColor(0.9f,1f,1f)][PropertySpace(5f)]
[TopTitle(
    NameSuffix = "<color=#00ffff33><b>▤</b></color>",
    NamePrefix = "<color=#00ffff33><b>▤</b></color>",
    ShowGenericName = true,SetTransform = true,SetParentObject = true,
    BoldName = true,HideNameOnMid = true,ShowNameOnPrefix = true,ShowTypeOnSuffix = true)]
public struct DataInstaller<T> : IDataInstaller
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
    [HorizontalGroup(GroupID = "install",Width = 0.99f)]
    [Button("Single")][HideIf("HideIfShowKey")]
    private void ShowKey()
    {
        _keyFieldToggled = true;
    }

    [PropertyOrder(-1)]
    [HorizontalGroup(GroupID = "install",Width = 0.05f)]
    [Button("X")][ShowIf("ShowIfHideKey")]
    private void HideKey()
    {
        _createBegun = false;
        _keyFieldToggled = false;
    }

    private bool HideIfShowKey => _keyFieldToggled || Key != null;
    private bool ShowIfHideKey => _keyFieldToggled && Key == null;
    
    [HideInPlayMode]
    [SerializeField]
    [HideLabel]
    [ValueDropdown("GetAllAppropriateKeys")]
    [HorizontalGroup(GroupID = "install",Width = 0.40f)]
    [HideIf("HideIfKey")]
    public DataKey Key;

    private bool HideIfKey => !_keyFieldToggled && Key == null;

    [SerializeField][HideLabel]
    public T InstalledValue;

    private bool _createBegun;
    private bool ShouldShowCreateDataSetKey => Key == null;
    public bool CreateBegun => _createBegun && ShouldShowCreateDataSetKey;
    public bool ShowBeginCreate => !_createBegun && ShouldShowCreateDataSetKey && _keyFieldToggled && Key == null;
    
    [Button("Create")][HorizontalGroup(GroupID = "install",Width = 0.15f)][ShowIf("ShowBeginCreate")][PropertyOrder(-1)]
    public void BeginCreate()
    {
        _createBegun = true;
    }
    [Button("Cancel")][HorizontalGroup(GroupID = "install",Width = 0.15f)][ShowIf("CreateBegun")][PropertyOrder(-1)]
    public void CancelCreate()
    {
        _createBegun = false;
    }

    [ShowInInspector] [HideLabel] [HorizontalGroup(GroupID = "creation", Width = 0.8f)] [ShowIf("CreateBegun")]
    private string _keyName;

    [Button("Create")][HorizontalGroup(GroupID = "creation",Width = 0.15f)][ShowIf("CreateBegun")][PropertyOrder(-1)]
    public void CreateDataSetKey()
    {
        Key = DataKey.CreateAtFolder<T>(_keyName);
        _createBegun = false;
        Undo.RecordObject(parentObject,"DictionaryKeyCreateAndSet");
        EditorUtility.SetDirty(parentObject);
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
    
    public void InstallFor(IDataContext main)
    {
        string key = Key ? Key.ID : "";
        if (InstalledValue == null && InstalledValue is not UnityEngine.Object)
        {
            InstalledValue = Activator.CreateInstance<T>();
        }
        DataRegistry<T>.SetData(main,InstalledValue,key);
    }
}