using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

//Responsible for integrating logic with data.
//First spark of logic.
[RequireComponent(typeof(IDataContext))]
[RequireComponent(typeof(IEventContext))]
[DisallowMultipleComponent]
public abstract class Actor : MonoBehaviour,IActor
{
    [SerializeField] private List<Key> _groups;
    [SerializeField] private bool _stopOnEnd;
    [SerializeField] private bool _setSceneReference;
    [ShowIf("_setSceneReference")] [SerializeField]
    private DataInstaller<IActor> _sceneInstall;
    
    [SerializeField][HideInPlayMode][ReadOnly] private GOInstance _goInstance;
    [SerializeField][HideInPlayMode][ReadOnly] private MonoBehaviour _dataContextObject;
    [SerializeField][HideInPlayMode][ReadOnly] private MonoBehaviour _eventContextObject;
    
    [ShowInInspector][HideInEditorMode]
    private IDataContext _dataContext;

    public string ObjectTypeID => _goInstance.ObjectTypeID;

    public IDataContext DataContext
    {
        get
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return null;
#endif
            if (!_isInitialized)
            {
                _dataContext = _dataContextObject as IDataContext;
            }

            return _dataContext;
        }
    }
    [ShowInInspector][HideInEditorMode]
    private IEventContext _eventContext;
    public IEventContext EventContext
    {
        get
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return null;
#endif
            if (!_isInitialized)
            {
                _eventContext = _eventContextObject as IEventContext;
            }

            return _eventContext;
        }
    }
    
    private bool _isInitialized;
    private IGOInstancePoolRegistry _goPool;
    public IGOInstancePoolRegistry GOPool => _goPool;
    public bool IsInitialized => _isInitialized;
    private bool _isRunning;
    public bool IsRunning => _isRunning;
    private bool _isEnded;
    public bool IsEnded => _isEnded;
    private string _endingEventID;
    public string EndingEventID => _endingEventID;
    public bool IsBeingDestroyed => _goInstance.IsBeingDestroyed;

    public event Action<IActor> onInitialize;
    public event Action<IActor> onBeginBeforeLogic;
    public event Action<IActor> onBegin;
    public event Action<IActor> onStop;
    public event Action<IActor> onDestroyActor;
    public event Action<IActor> onFinishEnded;
    public event Action<IActor> onCancelEnded;
    public event Action<IActor> onEnded; // Cancel or Finish
    public event Action<IActor> onEndedStateChanged;

    private bool ShowBeginButton => _isInitialized && !_isRunning;
    private bool ShowStopButton => _isInitialized && _isRunning;
    private bool ShowInitButton => !_isInitialized && Application.isPlaying;

    protected virtual void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (_goInstance == null)
            {
                _goInstance = GetComponent<IGOInstance>() as GOInstance;
            }
            if (_dataContextObject == null)
            {
                _dataContextObject = GetComponent<IDataContext>() as MonoBehaviour;
            }
            if (_eventContextObject == null)
            {
                _eventContextObject = GetComponent<IEventContext>() as MonoBehaviour;
            }
        }
    }

    protected void Awake()
    {
        foreach (Key group in _groups)
        {
            if (!DataRegistry<List<IActor>>.ContainsData("Global/"+group.ID))
            {
                DataRegistry<List<IActor>>.SetData(null,new List<IActor>(),group.ID);
            }

            DataRegistry<List<IActor>>.GetData(null, group.ID).Add(this);
        }
    }

    [Button][ShowIf("ShowInitButton")]
    public void InitializeIfNot()
    {
        if (_isInitialized) return;

        if (_setSceneReference)
        {
            _sceneInstall.InstalledValue = this;
            _sceneInstall.InstallFor(ContextRegistry.GetContext(gameObject.scene.name).As<IDataContext>());
        }
        _goPool = DataRegistry<IGOInstancePoolRegistry>.GetData(null);
        OnBeforeContextsInitialize();
        _dataContext = _dataContextObject as IDataContext;
        _dataContext.onDestroyContext += OnDestroyDataContext;
        _dataContext.InitializeIfNot();
        _dataContext.SetData(this as IActor);
        
        _eventContext = _eventContextObject as IEventContext;
        _eventContext.InitializeIfNot();
        OnAfterContextsInitialized();
        _isInitialized = true;
        onInitialize?.Invoke(this);
    }

    private void OnDestroyDataContext(IContext obj)
    {
        if (!_isInitialized) return;
        _dataContext.onDestroyContext -= OnDestroyDataContext;
        foreach (Key group in _groups)
        {
            DataRegistry<List<IActor>>.TryActionOnData(null,(a)=>a.Remove(this),group.ID);
        }

        if (_isRunning && !_onApplicationQuit)
        {
            StopIfNot();
        }
        onDestroyActor?.Invoke(this);
    }

    protected virtual void OnBeforeContextsInitialize()
    {
        
    }

    protected virtual void OnAfterContextsInitialized()
    {
        
    }

    [Button][ShowIf("ShowBeginButton")]
    public void BeginIfNot()
    {
        if (!_isInitialized) return;
        if (_isRunning) return;
        
        //Resets.
        _isEnded = false;
        
        onBeginBeforeLogic?.Invoke(this);
        OnBeginLogic();
        _isRunning = true;
        onBegin?.Invoke(this);
    }
    
    [Button][ShowIf("ShowStopButton")]
    public void FinishIfNotEnded(string eventID)
    {
        if (_isEnded) return;
        if (!_isRunning) return;
        _endingEventID = eventID;
        //Debug.Log("Checkout Finished, " + name + " - " + eventID);
        _isEnded = true;
        onFinishEnded?.Invoke(this);
        onEnded?.Invoke(this);
        if (_stopOnEnd)
        {
            StopIfNot();
        }
        onEndedStateChanged?.Invoke(this);
        DataContext.ParentContext = null;
    }
    
    [Button][ShowIf("ShowStopButton")]
    public void CancelIfNotEnded(string eventID)
    {
        if (_isEnded) return;
        if (!_isRunning) return;
        _endingEventID = eventID;
        _isEnded = true;
        onCancelEnded?.Invoke(this);
        onEnded?.Invoke(this);
        if (_stopOnEnd)
        {
            IActor main = this;
            main.StopIfNot();
        }
        onEndedStateChanged?.Invoke(this);
        DataContext.ParentContext = null;
    }

    protected virtual void OnBeginLogic()
    {
        
    }

    [Button][ShowIf("ShowStopButton")]
    public void StopIfNot()
    {
        if (!_isInitialized) return;
        if (!_isRunning) return;

        OnStopLogic();
        _isRunning = false;
        onStop?.Invoke(this);
        if (IsBeingDestroyed) return;
        _goInstance.ReturnPool();
    }

    protected virtual void OnStopLogic()
    {
        
    }

    private bool _onApplicationQuit;
    private void OnApplicationQuit()
    {
        _onApplicationQuit = true;
    }
}