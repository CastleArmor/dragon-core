using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(GOInstance))]
[DisallowMultipleComponent]
public class MonoContext : MonoBehaviour, IDataContext, IEventContext, IUnityComponent
{
    [SerializeField] private bool _isSceneContext;
    
    [ShowInInspector] [ReadOnly] private string _instanceID;
    public string EventContextID => _instanceID;
    public string DataContextID => _instanceID;

    [TabGroup("Settings",HideWhenChildrenAreInvisible = true)]
    [SerializeField] private DefaultInstallMethod _defaultInstallMethod;
    public virtual DataInstallMethod CreateInstallMethod()
    {
        return _defaultInstallMethod;
    }
    
    [TabGroup("Settings",HideWhenChildrenAreInvisible = true)]
    [SerializeField] 
    private GOInstance _goInstance;
    
    [SerializeField] 
    [TabGroup("Settings",HideWhenChildrenAreInvisible = true)]
    private bool _initializeAtAwake;
    
    [ShowInInspector] 
    [TabGroup("Outputs",HideWhenChildrenAreInvisible = true)] 
    [ReadOnly]
    private IDataContext _parentContext; 
    public IDataContext ParentContext
    {
        get => _parentContext;
        set
        {
            IDataContext oldValue = _parentContext;
            if (value != null)
            {
                if (value == (IDataContext)this)
                {
                    Debug.LogError("You cannot assign actor's self as user " + name);
                    return;
                }
            }
            bool isChanged = _parentContext != value;
            _parentContext = value;
            if (isChanged)
            {
                onParentContextChanged?.Invoke(this, oldValue, value);
            }
        }
    }

    [ShowInInspector] 
    [TabGroup("Outputs",HideWhenChildrenAreInvisible = true)] 
    [ReadOnly]
    private bool _isDataPrepared;
    public bool IsDataPrepared => _isDataPrepared;

    public bool IsDefaultPrefabInstance => _goInstance ? _goInstance.IsDefaultPrefabInstance : false;
    
    public bool IsPrefab => gameObject.IsPrefab();
    
    public event Action<IContext> onDestroyContext;

    public IDataContext RootContext
    {
        get
        {
            IDataContext latest = this;
            bool isFinal = false;
            while (!isFinal)
            {
                IDataContext iteration = latest.ParentContext;
                if (iteration == null)
                {
                    isFinal = true;
                }
                else
                {
                    latest = iteration;
                }
            }

            return latest;
        }
    }

    public event Action<IDataContext, IDataContext, IDataContext> onParentContextChanged;
    public event Action<IDataContext> onInitializeData;
    public event Action<IDataContext> onAllowAdditionalDataOnInitialize;
    public event Action<IDataContext> onRequestSaveData;
    public event Action<IDataContext> onRequestLoadData;
    public event Action<IDataContext> onDestroyDataContext;
    public event Action<IEventContext> onDestroyEventContext;

    public void SaveData()
    {
        onRequestSaveData?.Invoke(this);
        Debug.Log("Saving data completed...");
    }

    public void LoadData()
    {
        onRequestLoadData?.Invoke(this);
        Debug.Log("Loading data completed...");
    }

    private void Awake()
    {
        if (_initializeAtAwake)
        {
            InitializeIfNot();
        }
    }

    public void InitializeIfNot()
    {
        if (_isDataPrepared) return;
        _instanceID = _isSceneContext ? gameObject.scene.name : GetInstanceID().ToString();
        DataContextRegistry.Set(DataContextID,this);
        EventContextRegistry.Set(EventContextID,this);
        DataInstallMethod installMethod = CreateInstallMethod();
        installMethod.InstallFor(this);
        _isDataPrepared = true;
    }

    public T GetData<T>()
    {
        return DataRegistry<T>.GetData(this);
    }

    public T GetData<T>(string key)
    {
        return DataRegistry<T>.GetData(this, key);
    }

    public void SetData<T>(T value)
    {
        DataRegistry<T>.SetData(this,value);
    }

    public void SetData<T>(string key, T value)
    {
        DataRegistry<T>.SetData(this,value,key);
    }

    private void OnDestroy()
    {
        onDestroyContext?.Invoke(this);
        onDestroyDataContext?.Invoke(this);
        onDestroyEventContext?.Invoke(this);
        DataContextRegistry.Remove(this);
        EventContextRegistry.Remove(this);
    }
}