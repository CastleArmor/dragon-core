using System;
using UnityEngine;

[System.Serializable]
public class InstalledData : IInstalledData
{
    [NonSerialized]
    private IDataContext _context;
    public IDataContext Context => _context;
    
    [NonSerialized]
    private bool _isInstalled;
    public bool IsInstalled => _isInstalled;

    [NonSerialized]
    private bool _isInitializing;
    public bool IsInitializing => _isInitializing;

    [NonSerialized]
    private string _assignedID;
    public string AssignedID
    {
        get => _assignedID;
        set => _assignedID = value;
    }

    public void OnInstalledData(IDataContext context)
    {
        _isInitializing = true;
        _context = context;
        OnBindAdditional();
        OnInitialize();
        if (context != null)
        {
            if (!context.IsPrefab && !context.IsDefaultPrefabInstance)
            {
                OnInitializeInstanceData();
            }
        }

        _isInitializing = false;
        _isInstalled = true;
    }

    protected virtual void OnBindAdditional()
    {
        
    }

    protected virtual void OnUnbindAdditional()
    {
        
    }
    
    protected virtual void OnInitialize()
    {
        
    }

    protected virtual void OnInitializeInstanceData()
    {
        
    }

    public void OnRemoveData()
    {
        if (_context != null)
        {
            if (!_context.IsPrefab && !_context.IsDefaultPrefabInstance)
            {
                OnInitializeInstanceData();
            }
        }

        OnUnbindAdditional();
        OnRemove();
        _isInstalled = false;
    }
    
    protected virtual void OnRemove()
    {
        
    }
}