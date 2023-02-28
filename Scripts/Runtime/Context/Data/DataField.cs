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
using UnityEngine.SceneManagement;

public enum DataAddress
{
    Context = 0,
    Global = 1,
    GroupFirstMember = 2
}

public enum ContextAddress
{
    Self = 0,
    Parent=1,
    Root=2,
    Scene=3,
    Relative = 4
}

public enum RelativeAddress
{
    Self = 0,
    Parent = 1,
    Root = 2,
    Scene = 3
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
    
    private bool _keyFieldToggled;
    [PropertyOrder(-1)]
    [HorizontalGroup(GroupID = "install",Width = 0.40f)]
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

    private bool HideIfKey => !_keyFieldToggled && Key == null;
    
    [SerializeField]
    [HideLabel]
    [HorizontalGroup(GroupID = "install", Width = 0.30f)]
    public DataAddress DataAddress;
    
    [SerializeField]
    [HideLabel]
    [HideIf("HideIfContextAddress")]
    [HorizontalGroup(GroupID = "install", Width = 0.30f)]
    public ContextAddress ContextAddress;

    private bool HideIfContextAddress => DataAddress != DataAddress.Context;
    
    [SerializeField]
    [HideLabel]
    [HideIf("HideIfRelativeAddress")]
    [HorizontalGroup(GroupID = "relative", Width = 0.20f)]
    public RelativeAddress RelativeAddress;
    private bool HideIfRelativeAddress => ContextAddress != ContextAddress.Relative || HideIfContextAddress;

    [SerializeField]
    [HideLabel]
    [HideIf("HideIfRelativeAddress")]
    [HorizontalGroup(GroupID = "relative", Width = 0.80f)]
    public RelativeContextStack RelativeStack;

    [SerializeField]
    [HideLabel]
    [HideIf("HideIfGroupKey")]
    public DataKey GroupKey;
    private bool HideIfGroupKey => DataAddress != DataAddress.GroupFirstMember;

    [ShowInInspector][ReadOnly]
    private T _data;
    public T Data => _data;

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

    public void RegisterOnChange(IHierarchyContext context, Action<DataOnChangeArgs<T>> action)
    {
        DataRegistry<T>.RegisterOnChange(GetFromAddress(context), action, Key?Key.ID:"");
    }
    
    public void UnregisterOnChange(IHierarchyContext context, Action<DataOnChangeArgs<T>> action)
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

    public bool TryGet(IDataContext context)
    {
        string key = Key ? Key.ID : "";
        if (DataRegistry<T>.ContainsData(GetFromAddress(context), key))
        {
            _data = DataRegistry<T>.GetData(GetFromAddress(context),key);
            return true;
        }

        return false;
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

    private IContext GetFromAddress(IHierarchyContext context)
    {
        if (DataAddress == DataAddress.Global) return null;
        else
        {
            if (DataAddress == DataAddress.GroupFirstMember)
            {
                return DataRegistry<List<IActor>>.GetData(null, GroupKey.ID)[0].DataContext;
            }
            switch (ContextAddress)
            {
                case ContextAddress.Self: return context;
                case ContextAddress.Parent: return context.ParentContext;
                case ContextAddress.Root: return context.RootContext;
                case ContextAddress.Scene:
                    Scene scene = context.As<IUnityComponent>().gameObject.scene;
                    if (ContextRegistry.Contains(scene.name))
                    {
                        return ContextRegistry.GetContext(scene.name);
                    }
                    else return null;
                case ContextAddress.Relative:
                    switch (RelativeAddress)
                    {
                        case RelativeAddress.Self : 
                            return GetRelativeAtAddress(RelativeStack.ContextKeys, context);
                        case RelativeAddress.Parent :
                            return GetRelativeAtAddress(RelativeStack.ContextKeys, context.ParentContext);
                        case RelativeAddress.Root :
                            return GetRelativeAtAddress(RelativeStack.ContextKeys, context.RootContext);
                        case RelativeAddress.Scene :
                            Scene scene2 = context.As<IUnityComponent>().gameObject.scene;
                            return GetRelativeAtAddress(RelativeStack.ContextKeys, ContextRegistry.GetContext(scene2.name));
                    }
                    return context;
            }
            return context;
        }
    }
    
    public IContext GetRelativeAtAddress(List<DataKey> stack,IContext starting)
    {
        if (stack.Count == 0) return null;
        return RecursiveGetRelativeAtAddress(starting,stack, 0);
    }

    /// <summary>
    /// Returns only on initials.
    /// </summary>
    /// <param name="relationOwner"></param>
    /// <param name="stack"></param>
    /// <param name="currentIndex"></param>
    /// <returns></returns>
    private IContext RecursiveGetRelativeAtAddress(IContext relationOwner,List<DataKey> stack, int currentIndex)
    {
        if (stack.Count <= currentIndex)
        {
            return null;
        }
        if (relationOwner.ContainsData<IContext>(stack[currentIndex].ID))
        {
            IContext main = relationOwner.GetData<IContext>(stack[currentIndex].ID);
            IContext nextOwner = main;
            IContext recurse = RecursiveGetRelativeAtAddress(nextOwner,stack, currentIndex + 1);
            if (recurse != null)
            {
                main = recurse;
            }

            return main;
        }
        return null;
    }

    public static DataField<T> SingleContextRoot()
    {
        return new DataField<T>()
        {
            ContextAddress = ContextAddress.Root,
            DataAddress = DataAddress.Context
        };
    }
}