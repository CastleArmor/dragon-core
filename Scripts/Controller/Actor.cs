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
    [SerializeField][HideInPlayMode][ReadOnly] private GOInstance _goInstance;
    [SerializeField][HideInPlayMode][ReadOnly] private MonoBehaviour _dataContextObject;
    [SerializeField][HideInPlayMode][ReadOnly] private MonoBehaviour _eventContextObject;
    
    [ShowInInspector][HideInEditorMode]
    private IDataContext _dataContext;
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
    public event Action<IActor> onFinishEnded;
    public event Action<IActor> onCancelEnded;
    public event Action<IActor> onEnded; // Cancel or Finish
    public event Action<IActor> onEndedStateChanged;
    public event Action<IActor,string> onRequestCancel;

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
        
        _goPool = DataRegistry<IGOInstancePoolRegistry>.GetData(null);
        OnBeforeContextsInitialize();
        _dataContext = _dataContextObject as IDataContext;
        _dataContext.InitializeIfNot();
        _dataContext.SetData(this as IActor);
        
        _eventContext = _eventContextObject as IEventContext;
        _eventContext.InitializeIfNot();
        OnAfterContextsInitialized();
        _isInitialized = true;
        onInitialize?.Invoke(this);
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
    
    //Designed to called by owner of this
    public void CheckoutFinished(string eventID)
    {
        if (_isEnded) return;
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
    }
    
    //Designed to called by owner of this
    public void CheckoutCancelled(string eventID)
    {
        if (_isEnded) return;
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
    }
    
    public void Cancel(string requestID)
    {
        onRequestCancel?.Invoke(this,requestID);
        CheckoutCancelled(requestID);
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
        _goInstance.ReturnPool();
    }

    protected virtual void OnStopLogic()
    {
        
    }

    private void OnDestroy()
    {
        if (!_isInitialized) return;
        foreach (Key group in _groups)
        {
            DataRegistry<List<IActor>>.TryActionOnData(null,(a)=>a.Remove(this),group.ID);
        }

        if (_isRunning)
        {
            StopIfNot();
        }
    }
}