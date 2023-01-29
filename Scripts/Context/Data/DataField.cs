using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DataAddress
{
    Global = 0,
    Context = 1,
    ParentContext=2,
    RootContext=3,
    SceneContext=4
}

[System.Serializable][TopTitle(
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
    
    [HideInPlayMode]
    [SerializeField]
    [HideLabel]
    [ValueDropdown("GetAllAppropriateKeys")]
    [HorizontalGroup(GroupID = "install",Width = 0.40f)]
    public DataKey Key;

    [HideInPlayMode]
    [SerializeField]
    [HideLabel]
    [HorizontalGroup(GroupID = "install", Width = 0.40f)]
    public DataAddress DataAddress;

    [ShowInInspector][ReadOnly]
    private T _data;
    public T Data => _data;

    private bool _createBegun;
    private bool ShouldShowCreateDataSetKey => Key == null;
    public bool CreateBegun => _createBegun && ShouldShowCreateDataSetKey;
    public bool ShowBeginCreate => !_createBegun && ShouldShowCreateDataSetKey;
    
    [Button("Create")][HorizontalGroup(GroupID = "install",Width = 0.2f)][ShowIf("ShowBeginCreate")]
    public void BeginCreate()
    {
        _createBegun = true;
    }

    [ShowInInspector] [HideLabel] [HorizontalGroup(GroupID = "creation", Width = 0.8f)] [ShowIf("CreateBegun")]
    private string _keyName;

    [Button("Create")][HorizontalGroup(GroupID = "creation",Width = 0.1f)][ShowIf("CreateBegun")]
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

    public void RegisterOnChange(IDataContext context, Action<DataOnChangeArgs<T>> action)
    {
        DataRegistry<T>.RegisterOnChange(GetFromAddress(context), action, Key?Key.ID:"");
    }
    
    public void UnregisterOnChange(IDataContext context, Action<DataOnChangeArgs<T>> action)
    {
        DataRegistry<T>.UnregisterOnChange(GetFromAddress(context), action, Key?Key.ID:"");
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
    
    public T Get(IDataContext context)
    {
        string key = Key ? Key.ID : "";
        _data = DataRegistry<T>.GetData(GetFromAddress(context),key);
        return _data;
    }

    [Button]
    public void Set(IDataContext context, T value)
    {
        string key = Key ? Key.ID : "";
        DataRegistry<T>.SetData(GetFromAddress(context),value,key);
    }

    private IDataContext GetFromAddress(IDataContext context)
    {
        if (DataAddress == DataAddress.Global) return null;
        else
        {
            switch (DataAddress)
            {
                case DataAddress.Context: return context;
                case DataAddress.ParentContext: return context.ParentContext;
                case DataAddress.RootContext: return context.RootContext;
                case DataAddress.SceneContext :
                    Scene scene = context.As<IUnityComponent>().gameObject.scene;
                    return DataContextRegistry.GetContext(scene.name);
            }
            return context;
        }
    }
}